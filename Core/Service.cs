using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Greatbone.Core
{
    ///
    /// A service work implements a microservice.
    ///
    public abstract class Service : Folder, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the service instance id
        readonly string moniker;

        // the embedded server
        readonly KestrelServer server;

        // event consumption
        readonly Roll<EventInfo> events;

        // client connectivity to the related peers
        readonly Roll<Client> clients;

        // event providing
        readonly Roll<EventQueue> queues;

        readonly ResponseCache cache;

        Thread scheduler;

        Thread cleaner;

        protected Service(ServiceContext sc) : base(sc)
        {
            // adjust configuration
            sc.Service = this;

            moniker = (sc.shard == null) ? sc.name : sc.name + "-" + sc.shard;

            // setup logging 
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = sc.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false)
            {
                AutoFlush = true
            };

            // create kestrel instance
            KestrelServerOptions options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), Lifetime, factory);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (string.IsNullOrEmpty(sc.addresses))
            {
                throw new ServiceException("missing 'addresses'");
            }
            foreach (string a in sc.addresses.Split(',', ';'))
            {
                addrs.Add(a.Trim());
            }

            // initialize event handlers
            Type typ = GetType();
            foreach (MethodInfo mi in typ.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                // verify the return type
                Type ret = mi.ReturnType;
                bool async;
                if (ret == typeof(Task)) async = true;
                else if (ret == typeof(void)) async = false;
                else continue;

                ParameterInfo[] pis = mi.GetParameters();
                EventInfo evt;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(EventContext))
                {
                    evt = new EventInfo(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(EventContext) && pis[1].ParameterType == typeof(string))
                {
                    evt = new EventInfo(this, mi, async, true);
                }
                else continue;

                if (events == null)
                {
                    events = new Roll<EventInfo>(16);
                }
                events.Add(evt);
            }

            // initialize connectivities to the cluster members
            Dictionary<string, string> cluster = sc.cluster;
            if (cluster != null)
            {
                foreach (KeyValuePair<string, string> mem in cluster)
                {
                    if (clients == null)
                    {
                        clients = new Roll<Client>(cluster.Count * 2);
                    }
                    clients.Add(new Client(mem.Key, mem.Value));

                    if (queues == null)
                    {
                        queues = new Roll<EventQueue>(cluster.Count * 2);
                    }
                    queues.Add(new EventQueue(mem.Key, 20));
                }
                EventQueue.GlobalInit(this, queues);
            }

            // initialize response cache
            cache = new ResponseCache(Environment.ProcessorCount * 2, 4096);

        }

        public string Describe()
        {
            XmlContent cont = new XmlContent(false, false);
            Describe(cont);
            return cont.ToString();
        }

        ///
        /// Uniquely identify a service instance.
        ///
        public string Moniker => moniker;

        public Roll<EventInfo> Events => events;

        public Roll<Client> Cluster => clients;

        public new ServiceContext Context => (ServiceContext)context;

        internal ResponseCache Cache => cache;

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        //
        // authentication
        //
        protected virtual void Authenticate(ActionContext ac) { }

        protected virtual void Challenge(ActionContext ac) { }


        ///  
        /// Returns a framework custom context.
        /// 
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new ActionContext(features)
            {
                ServiceContext = Context
            };
        }


        /// 
        /// To asynchronously process the request.
        /// 
        public async Task ProcessRequestAsync(HttpContext context)
        {
            ActionContext ac = (ActionContext)context;
            HttpRequest req = ac.Request;
            string path = req.Path.Value;

            try // authentication
            {
                Authenticate(ac);
            }
            catch (Exception e)
            {
                DBG(e.Message);
            }

            try
            {
                if ("/*".Equals(path)) // handle an event queue request
                {
                    using (var dc = NewDbContext())
                    {
                        // EventsUtility.PeekEq();
                    }
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Folder folder = ResolveFolder(ref relative, ac);
                    if (folder == null)
                    {
                        ac.Reply(404); // not found
                        return;
                    }
                    await folder.HandleAsync(relative, ac);
                }
            }
            catch (Exception e)
            {
                if (e is ParseException)
                {
                    ac.Reply(400, e.Message); // bad request
                }
                else if (e is AccessException)
                {
                    if (ac.Token == null) { Challenge(ac); }
                    else
                    {
                        ac.Reply(403); // forbidden
                    }
                }
                else
                {
                    DBG(e.Message, e);
                    ac.Reply(500, e.Message);
                }
            }

            // prepare and send
            try
            {
                await ac.SendAsync();
            }
            catch (Exception e)
            {
                ERR(e.Message, e);
                ac.Reply(500, e.Message);
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            // dispose the action context
            ((ActionContext)context).Dispose();
        }


        public DbContext NewDbContext(IsolationLevel? level = null)
        {
            DbContext dc = new DbContext(Context);
            if (level != null)
            {
                dc.Begin(level.Value);
            }
            return dc;
        }

        //
        // CLUSTER
        //

        internal Client GetClient(string svcid)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                Client cli = clients[i];
                if (cli.Name.Equals(svcid)) return cli;
            }
            return null;
        }

        public void Start()
        {
            // start the server
            //
            server.Start(this);

            OnStart();

            Debug.WriteLine(Name + " -> " + Context.addresses + " started");

            // start helper threads

            cleaner = new Thread(Clean);
            // cleaner.Start();

            if (clients != null)
            {
                scheduler = new Thread(Schedule);
                // scheduler.Start();
            }
        }

        ///
        /// Run in the scheduler thread to repeatedly check and initiate event polling activities.
        internal void Schedule()
        {
            while (!stop)
            {
                Thread.Sleep(50);

                int tick = Environment.TickCount;
                for (int i = 0; i < Cluster.Count; i++)
                {
                    Client client = Cluster[i];
                    client.ToPoll(tick);
                }
            }
        }

        volatile bool stop;

        ///
        /// Run in the cleaner thread to repeatedly check and relinguish cache entries.
        internal void Clean()
        {
            while (!stop)
            {
                Thread.Sleep(1000);

                int now = Environment.TickCount;
            }
        }
        //
        // LOGGING
        //

        // sub controllers are already there
        public ILogger CreateLogger(string name)
        {
            return this;
        }

        // opened writer on the log file
        readonly StreamWriter logWriter;

        public IDisposable BeginScope<T>(T state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel level)
        {
            return (int)level >= Context.logging;
        }

        public void Dispose()
        {
            server.Dispose();

            logWriter.Flush();
            logWriter.Dispose();

            Console.Write(Name);
            Console.WriteLine(".");
        }

        static readonly string[] LVL = { "TRC: ", "DBG: ", "INF: ", "WAR: ", "ERR: ", "CRL: ", "NON: " };

        public void Log<T>(LogLevel level, EventId eid, T state, Exception exception, Func<T, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            logWriter.Write(LVL[(int)level]);

            if (eid.Id != 0)
            {
                logWriter.Write("{");
                logWriter.Write(eid.Id);
                logWriter.Write("} ");
            }

            if (formatter != null) // custom format
            {
                var message = formatter(state, exception);
                logWriter.WriteLine(message);
            }
            else // fixed format
            {
                logWriter.WriteLine(state.ToString());
                if (exception != null)
                {
                    logWriter.WriteLine(exception.StackTrace);
                }
            }
        }

        ///
        /// STATIC
        ///
        static readonly ApplicationLifetime Lifetime = new ApplicationLifetime();


        /// 
        /// Runs a number of web services and block until shutdown.
        /// 
        public static void Run(IEnumerable<Service> services)
        {
            using (var cts = new CancellationTokenSource())
            {
                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    cts.Cancel();

                    // wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                // start services
                var svcs = services as Service[] ?? services.ToArray();
                foreach (Service svc in svcs)
                {
                    svc.Start();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                    {
                        ((IApplicationLifetime)state).StopApplication();
                        // dispose services
                        foreach (Service svc in svcs)
                        {
                            svc.OnStop();

                            svc.Dispose();
                        }
                    },
                    Lifetime);

                Lifetime.ApplicationStopping.WaitHandle.WaitOne();
            }
        }
    }

    ///
    /// A microservice that implements authentication and authorization.
    ///
    public abstract class Service<TToken> : Service where TToken : IData, new()
    {
        protected Service(ServiceContext sc) : base(sc) { }

        protected override void Authenticate(ActionContext ac)
        {
            string tokstr;
            string hv = ac.Header("Authorization");
            if (hv != null && hv.StartsWith("Bearer ")) // the Bearer scheme
            {
                tokstr = hv.Substring(7);
                string jsonstr = Context.Decrypt(tokstr);
                ac.Token = JsonUtility.StringToObject<TToken>(jsonstr);
            }
            else if (ac.Cookies.TryGetValue("Bearer", out tokstr))
            {
                string jsonstr = Context.Decrypt(tokstr);
                ac.Token = JsonUtility.StringToObject<TToken>(jsonstr);
            }
        }

        protected override void Challenge(ActionContext ac)
        {
            string ua = ac.Header("User-Agent");
            if (ua.Contains("Mozila")) // browser
            {
                string loc = "singon" + "?orig=" + ac.Uri;
                ac.SetHeader("Location", loc);
                ac.Reply(303); // see other - redirect to signon url
            }
            else // non-browser
            {
                ac.SetHeader("WWW-Authenticate", "Bearer");
                ac.Reply(401); // unauthorized
            }
        }
    }
}