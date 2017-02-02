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
    public abstract class WebService : WebFolder, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the service instance id
        readonly string key;

        // the embedded server
        readonly KestrelServer server;

        // client connectivity to the related peers
        readonly Roll<WebClient> cluster;

        // event hooks
        readonly Roll<WebEvent> events;

        readonly WebCache cache;

        Thread scheduler;

        Thread cleaner;

        protected WebService(WebServiceContext sc) : base(sc)
        {
            // adjust configuration
            sc.Service = this;

            key = (sc.shard == null) ? sc.name : sc.name + "-" + sc.shard;

            // setup logging 
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = sc.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false)
            {
                AutoFlush = true
            };

            // create embedded server instance
            KestrelServerOptions options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), Lifetime, factory);
            ICollection<string> addrcoll = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (string.IsNullOrEmpty(sc.addresses))
            {
                throw new WebServiceException("'addresss' in webconfig");
            }
            foreach (string a in sc.addresses.Split(',', ';'))
            {
                addrcoll.Add(a.Trim());
            }

            // init event hooks
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
                WebEvent evt = null;
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebEventContext))
                {
                    evt = new WebEvent(this, mi, async, false);
                }
                else if (pis.Length == 2 && pis[0].ParameterType == typeof(WebEventContext) && pis[1].ParameterType == typeof(string))
                {
                    evt = new WebEvent(this, mi, async, true);
                }
                else continue;

                if (events == null)
                {
                    events = new Roll<WebEvent>(16);
                }
                events.Add(evt);
            }

            // init refers
            Dictionary<string, string> refs = sc.cluster;
            if (refs != null)
            {
                foreach (KeyValuePair<string, string> e in refs)
                {
                    if (cluster == null)
                    {
                        cluster = new Roll<WebClient>(refs.Count * 2);
                    }
                    cluster.Add(new WebClient(e.Key, e.Value));
                }
            }

            // init response cache
            cache = new WebCache(Environment.ProcessorCount * 2, 4096);

            // create database structures for event queue
            if (Context.db.queue)
            {
                CreateEq();
            }

        }

        public void Tree()
        {
            Debug.Write("service");
            Debug.Write(Name);
            if (subs != null)
            {
                for (int i = 0; i < subs.Count; i++)
                {
                    WebFolder child = subs[i];
                    Debug.Write("SUB " + child.Name);
                }
            }
            for (int i = 0; i < Actions.Count; i++)
            {
                WebAction action = Actions[i];
                Debug.Write("ACT " + action.Name);
            }
        }

        ///
        /// Uniquely identify a service instance.
        ///
        public string Key => key;

        public Roll<WebEvent> Events => events;

        public Roll<WebClient> Cluster => cluster;

        public new WebServiceContext Context => (WebServiceContext)context;

        public WebAuthent Authent { get; set; }

        internal WebCache Cache => cache;

        bool CreateEq()
        {
            // check db
            using (var dc = Service.NewDbContext())
            {
                dc.Execute(@"CREATE TABLE IF NOT EXISTS eu (
                                moniker varchar(20),
                                lastid int8,
                                CONSTRAINT eu_pkey PRIMARY KEY (moniker)
                            ) WITH (OIDS=FALSE)"
                );

                dc.Execute(@"CREATE SEQUENCE IF NOT EXISTS eq_id_seq 
                                INCREMENT 1 
                                MINVALUE 1 MAXVALUE 9223372036854775807 
                                START 1 CACHE 32 NO CYCLE
                                OWNED BY eq.id"
                );

                dc.Execute(@"CREATE TABLE IF NOT EXISTS eq (
                                id int8 DEFAULT nextval('eq_id_seq'::regclass) NOT NULL,
                                name varchar(40),
                                shard varchar(20),
                                time timestamp,
                                type varchar(40),
                                body bytea,
                                CONSTRAINT eq_pkey PRIMARY KEY (id)
                            ) WITH (OIDS=FALSE)"
                );

            }
            return true;
        }

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
            return new WebActionContext(features);
        }

        /// 
        /// To asynchronously process the request.
        /// 
        public async Task ProcessRequestAsync(HttpContext context)
        {
            WebActionContext ac = (WebActionContext)context;
            HttpRequest req = ac.Request;
            string path = req.Path.Value;

            // authentication
            if (Authent != null)
            {
                try
                {
                    Authent.Authenticate(ac);
                }
                catch (Exception e)
                {
                    DBG(e.Message);
                }
            }

            try
            {
                if ("/*".Equals(path)) // handle an event queue request
                {
                    PeekEq(ac);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    WebFolder folder = Locate(ref relative, ac);
                    if (folder != null)
                    {
                        await folder.HandleAsync(relative, ac);
                    }
                }
            }
            catch (Exception e)
            {
                if (e is ParseException)
                {
                    ac.Reply(400, e.Message); // bad request
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
            ((WebActionContext)context).Dispose();
        }


        public DbContext NewDbContext(IsolationLevel level = IsolationLevel.Unspecified)
        {
            return new DbContext(Context)
            {
                Transact = level
            };
        }

        //
        // CLUSTER
        //

        internal WebClient GetClient(string svcid)
        {
            for (int i = 0; i < cluster.Count; i++)
            {
                WebClient cli = cluster[i];
                if (cli.Name.Equals(svcid)) return cli;
            }
            return null;
        }


        ///
        /// Peek and load events from the event queue (DB)
        ///
        void PeekEq(WebActionContext ac)
        {
            string[] names = ac[nameof(names)];
            string shard = ac.Header("Shard"); // can be null
            long? lastid = ac.HeaderLong("Range");

            using (var dc = NewDbContext())
            {
                DbSql sql = new DbSql("SELECT id, name, time, type, body FROM eq WHERE id > @1 AND event IN [");
                for (int i = 0; i < names.Length; i++)
                {
                    if (i > 0) sql.Add(',');
                    sql.Put(null, names[i]);
                }
                sql.Add(']');
                if (shard != null)
                {
                    // the IN clause with shard is normally fixed, don't need to be parameters
                    sql._("AND (shard IS NULL OR shard =")._(shard)._(")");
                }
                sql._("LIMIT 120");

                if (dc.Query(sql, p => p.Set(lastid.Value)))
                {
                    FormMpContent cont = new FormMpContent(true, 1024 * 1024);
                    while (dc.Next())
                    {
                        long id = dc.GetLong();
                        string name = dc.GetString();
                        DateTime time = dc.GetDateTime();
                        string type = dc.GetString();
                        ArraySegment<byte> body = dc.GetBytesSeg();

                        // add an extension part
                        cont.PutEvent(id, name, time, type, body);
                    }
                    ac.Reply(200, cont);
                }
                else
                {
                    ac.Reply(204); // no content
                }
            }
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

            if (cluster != null)
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
                    WebClient client = Cluster[i];
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
        static readonly WebLifetime Lifetime = new WebLifetime();


        /// 
        /// Runs a number of web services and block until shutdown.
        /// 
        public static void Run(IEnumerable<WebService> services)
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
                var svcs = services as WebService[] ?? services.ToArray();
                foreach (WebService svc in svcs)
                {
                    svc.Start();
                }

                Console.WriteLine("ctrl_c to shut down");

                cts.Token.Register(state =>
                    {
                        ((IApplicationLifetime)state).StopApplication();
                        // dispose services
                        foreach (WebService svc in svcs)
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
}