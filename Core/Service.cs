using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    /// A service is a HTTP endpoint that manages resources in a hierarchical manner.
    ///
    public abstract class Service : Folder, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        protected readonly ServiceContext sc;

        // the service instance id
        readonly string id;

        // the embedded server
        readonly KestrelServer server;

        // event consumption
        readonly Roll<EventInfo> events;

        // client connectivity to the related peers
        readonly Roll<Client> clients;

        // event providing
        readonly Roll<EventQueue> queues;

        readonly ActionCache cache;

        Thread scheduler;

        Thread cleaner;

        protected Service(ServiceContext sc) : base(sc)
        {
            sc.Service = this;
            this.sc = sc;

            id = (Shard == null) ? sc.Name : sc.Name + "-" + Shard;

            // setup logging 
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = sc.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false) { AutoFlush = true };

            // create kestrel instance
            KestrelServerOptions options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), Application.Lifetime, factory);
            ICollection<string> addrcoll = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (Addrs == null)
            {
                throw new ServiceException("missing 'addrs'");
            }
            foreach (string a in Addrs)
            {
                addrcoll.Add(a.Trim());
            }

            // events
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

            // cluster connectivity
            if (Cluster != null)
            {
                foreach (KeyValuePair<string, string> entry in Cluster)
                {
                    if (clients == null)
                    {
                        clients = new Roll<Client>(Cluster.Count * 2);
                    }
                    clients.Add(new Client(this, entry.Key, entry.Value));

                    if (queues == null)
                    {
                        queues = new Roll<EventQueue>(Cluster.Count * 2);
                    }
                    queues.Add(new EventQueue(entry.Key));
                }
            }

            // response cache
            cache = new ActionCache(Environment.ProcessorCount * 2, 4096);

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
        public string Id => id;

        public Roll<Client> Clients => clients;

        public Roll<EventInfo> Events => events;

        internal ActionCache Cache => cache;

        public string Shard => sc.shard;

        public string[] Addrs => sc.addrs;

        public Db Db => sc.db;

        public Map Cluster => sc.cluster;

        public int Logging => sc.logging;


        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        ///  
        /// Returns a framework custom context.
        /// 
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new ActionContext(features)
            {
                Service = this
            };
        }


        /// 
        /// To asynchronously process the request.
        /// 
        public virtual async Task ProcessRequestAsync(HttpContext context)
        {
            ActionContext ac = (ActionContext)context;
            HttpRequest req = ac.Request;
            string path = req.Path.Value;

            try
            {
                if ("/*".Equals(path)) // handle an event poll request
                {
                    Poll(ac);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Folder folder = ResolveFolder(ref relative, ac);
                    if (folder == null)
                    {
                        ac.Give(404); // not found
                        return;
                    }

                    await folder.HandleAsync(relative, ac);
                }
            }
            catch (ParseException e)
            {
                ac.Give(400, e.Message); // bad request
            }
            catch (Exception e)
            {
                DBG(e.Message, e);
                ac.Give(500, e.Message);
            }


            // prepare and send
            try
            {
                await ac.SendAsync();
            }
            catch (Exception e)
            {
                ERR(e.Message, e);
                ac.Give(500, e.Message);
            }
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            // dispose the action context
            ((ActionContext)context).Dispose();
        }


        public DbContext NewDbContext(IsolationLevel? level = null)
        {
            DbContext dc = new DbContext(this);
            if (level != null)
            {
                dc.Begin(level.Value);
            }
            return dc;
        }

        protected void Poll(ActionContext ac)
        {
            if (queues == null)
            {
                ac.Give(501); // not implemented
            }
            else
            {
                EventQueue eq;
                string from = ac.Header("From");
                if (from == null || (eq = queues[from]) == null)
                {
                    ac.Give(400); // bad request
                }
                else
                {
                    eq.Poll(ac);
                }
            }

        }

        volatile string connstr;

        public string ConnectionString
        {
            get
            {
                if (connstr == null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Host=").Append(Db.host);
                    sb.Append(";Port=").Append(Db.port);
                    sb.Append(";Database=").Append(Db.database ?? Name);
                    sb.Append(";Username=").Append(Db.username);
                    sb.Append(";Password=").Append(Db.password);
                    sb.Append(";Read Buffer Size=").Append(1024 * 32);
                    sb.Append(";Write Buffer Size=").Append(1024 * 32);
                    sb.Append(";No Reset On Close=").Append(true);

                    connstr = sb.ToString();
                }
                return connstr;
            }
        }

        //
        // CLUSTER
        //

        internal Client GetClient(string moniker)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                Client cli = clients[i];
                if (cli.Name.Equals(moniker)) return cli;
            }
            return null;
        }

        volatile bool stop;

        public void Start()
        {
            if (clients != null)
            {
                EventQueue.GlobalInit(this, clients);
            }

            // start the server
            //
            server.Start(this);

            OnStart();

            DBG(Name + " -> " + Addrs[0] + " started");

            /// Run in the cleaner thread to repeatedly check and relinguish cache entries.
            cleaner = new Thread(() =>
            {
                while (!stop)
                {
                    Thread.Sleep(1000);

                    int now = Environment.TickCount;
                }
            });
            // cleaner.Start();

            if (clients != null)
            {
                /// Run in the scheduler thread to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (!stop)
                    {
                        // interval
                        Thread.Sleep(5000);

                        // a schedule cycle
                        int tick = Environment.TickCount;
                        for (int i = 0; i < Clients.Count; i++)
                        {
                            Client client = Clients[i];
                            client.TryPoll(tick);
                        }
                    }
                });
                // scheduler.Start();
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
            return (int)level >= Logging;
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

    }

    ///
    /// A microservice that implements authentication and authorization.
    ///
    public abstract class Service<TPrin> : Service where TPrin : class, IData, new()
    {
        protected Service(ServiceContext sc) : base(sc)
        {
        }

        public Auth Auth => sc.auth;

        public bool Async { get; set; }

        protected virtual void Authenticate(ActionContext ac)
        {
            string token;
            string hv = ac.Header("Authorization");
            if (hv != null && hv.StartsWith("Bearer ")) // the Bearer scheme
            {
                token = hv.Substring(7);
                ac.Principal = Decrypt(token);
            }
            else if (ac.Cookies.TryGetValue("Bearer", out token))
            {
                ac.Principal = Decrypt(token);
            }
        }

        protected virtual async Task AuthenticateAsync(ActionContext ac)
        {
            await Task.CompletedTask;
        }

        protected virtual void GiveChallenge(ActionContext ac)
        {
            string ua = ac.Header("User-Agent");
            if (ua.Contains("Mozila")) // if browser
            {
                string loc = "singon" + "?orig=" + ac.Uri;
                ac.SetHeader("Location", loc);
                ac.Give(303); // see other - redirect to signon url
            }
            else // non-browser
            {
                ac.SetHeader("WWW-Authenticate", "Bearer");
                ac.Give(401); // unauthorized
            }
        }

        /// 
        /// To asynchronously process the request.
        /// 
        public virtual async Task ProcessRequestAsync(HttpContext context)
        {
            ActionContext ac = (ActionContext)context;
            HttpRequest req = ac.Request;
            string path = req.Path.Value;

            try // authentication
            {
                if (Async) { await AuthenticateAsync(ac); }
                else { Authenticate(ac); }
            }
            catch (Exception e) { DBG(e.Message); }

            try
            {
                if ("/*".Equals(path)) // handle an event poll request
                {
                    Poll(ac);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Folder folder = ResolveFolder(ref relative, ac);
                    if (folder == null)
                    {
                        ac.Give(404); // not found
                        return;
                    }

                    await folder.HandleAsync(relative, ac);
                }
            }
            catch (ParseException e)
            {
                ac.Give(400, e.Message); // bad request
            }
            catch (AuthorizeException)
            {
                if (ac.Principal == null)
                {
                    GiveChallenge(ac);
                }
                else
                {
                    ac.Give(403); // forbidden
                }
            }
            catch (Exception e)
            {
                DBG(e.Message, e);
                ac.Give(500, e.Message);
            }


            // prepare and send
            try
            {
                await ac.SendAsync();
            }
            catch (Exception e)
            {
                ERR(e.Message, e);
                ac.Give(500, e.Message);
            }
        }

        public void SetTokenCookie(ActionContext ac, TPrin principal)
        {
            StringBuilder sb = new StringBuilder("Bearer=");
            string token = Encrypt(principal);
            sb.Append(token);
            if (Auth.maxage > 0)
            {
                sb.Append("; Max-Age=").Append(Auth.maxage);
            }
            if (Auth.domain != null)
            {
                sb.Append("; Domain=").Append(Auth.domain);
            }
            sb.Append("; HttpOnly");
            ac.SetHeader("Set-Cookie", sb.ToString());
        }

        public string Encrypt(TPrin principal)
        {
            if (Auth == null) return null;

            JsonContent cont = new JsonContent(true, true, 4096); // borrow
            cont.Put(null, principal);
            byte[] bytebuf = cont.ByteBuffer;
            int count = cont.Size;

            int mask = Auth.mask;
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            char[] charbuf = new char[count * 2]; // the target 
            int p = 0;
            for (int i = 0; i < count; i++)
            {
                // masking
                int b = bytebuf[i] ^ masks[i % 4];

                //transform
                charbuf[p++] = HEX[(b >> 4) & 0x0f];
                charbuf[p++] = HEX[(b) & 0x0f];

                // reordering
            }
            // return pool
            BufferUtility.Return(bytebuf);

            return new string(charbuf, 0, charbuf.Length);
        }

        public TPrin Decrypt(string token)
        {
            if (Auth == null) return null;

            int mask = Auth.mask;
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            int len = token.Length / 2;
            Text str = new Text(256);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform to byte
                int b = (byte)(Dv(token[p++]) << 4 | Dv(token[p++]));

                // masking
                str.Accept((byte)(b ^ masks[i % 4]));
            }

            JsonParse parse = new JsonParse(str.ToString());
            JObj jo = (JObj)parse.Parse();
            return jo.ToObject<TPrin>();
        }

        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        // return digit value
        static int Dv(char hex)
        {
            int v = hex - '0';
            if (v >= 0 && v <= 9)
            {
                return v;
            }
            v = hex - 'A';
            if (v >= 0 && v <= 5) return 10 + v;
            return 0;
        }
    }
}