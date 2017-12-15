using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
    /// <summary>
    /// A service is a work that implements HTTP endpoint.
    /// </summary>
    public abstract class Service : Work, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the service instance id
        readonly string id;

        // the embedded server
        readonly KestrelServer server;

        // event consumption
        readonly Map<string, EventInfo> events;

        // clients to clustered peers
        readonly Map<string, Client> clients;

        // event queues
        readonly Map<string, EventQueue> queues;

        // event schesuler thread
        Thread scheduler;

        // cache entries
        readonly ConcurrentDictionary<string, Cachie> cachies;

        // cache cleaner thread
        readonly Thread cleaner;

        protected Service(ServiceContext sc) : base(sc)
        {
            id = (ServiceCtx.shard == null) ? sc.Name : sc.Name + "-" + ServiceCtx.shard;

            // setup logging
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = sc.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false) {AutoFlush = true};

            // create kestrel instance
            KestrelServerOptions options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), ServiceUtility.Lifetime, factory);
            ICollection<string> addrcoll = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (ServiceCtx.addrs == null)
            {
                throw new ServiceException("missing 'addrs'");
            }
            foreach (string a in ServiceCtx.addrs)
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
                    events = new Map<string, EventInfo>(16);
                }
                events.Add(evt.Key, evt);
            }

            // cluster connectivity
            var cluster = ServiceCtx.cluster;
            if (cluster != null)
            {
                for (int i = 0; i < cluster.Count; i++)
                {
                    var e = ServiceCtx.cluster.At(i);
                    if (clients == null)
                    {
                        clients = new Map<string, Client>(cluster.Count * 2);
                    }
                    clients.Add(new Client(this, e.Key, e.Value));

                    if (queues == null)
                    {
                        queues = new Map<string, EventQueue>(cluster.Count * 2);
                    }
                    queues.Add(e.Key, new EventQueue(e.Key));
                }
            }

            // response cache
            cleaner = new Thread(Clean) {Name = "Cleaner"};
            cachies = new ConcurrentDictionary<string, Cachie>(Environment.ProcessorCount * 2, 1024);
        }

        public string Describe()
        {
            XmlContent cont = new XmlContent(false);
            Describe(cont);
            return cont.ToString();
        }

        ///
        /// Uniquely identify a service instance.
        ///
        public string Id => id;

        public Map<string, Client> Clients => clients;

        public Map<string, EventInfo> Events => events;

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            WAR(e.Message, e);
            ac.Give(500, e.Message);
        }

        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public virtual async Task ProcessRequestAsync(HttpContext context)
        {
            ActionContext ac = (ActionContext) context;
            HttpRequest req = ac.Request;
            string path = req.Path.Value;

            // handling
            try
            {
                if ("/*".Equals(path)) // handle an event poll request
                {
                    Poll(ac);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Work work = Resolve(ref relative, ac);
                    if (work == null)
                    {
                        ac.Give(404); // not found
                        return;
                    }
                    await work.HandleAsync(relative, ac);
                }
            }
            catch (Exception e)
            {
                Catch(e, ac);
            }
            // sending
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

        ///
        /// Returns a framework custom context.
        ///
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new ActionContext(features)
            {
                ServiceCtx = ServiceCtx
            };
        }

        public void DisposeContext(HttpContext context, Exception exception)
        {
            // dispose the action context
            ((ActionContext) context).Dispose();
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

        //
        // CACHE

        internal void Clean()
        {
            while (!stop)
            {
                Thread.Sleep(1000 * 30); // 30 seconds

                int now = Environment.TickCount;

                // a single loop to clean up expired items
                using (var enm = cachies.GetEnumerator())
                {
                    while (enm.MoveNext())
                    {
                        Cachie ca = enm.Current.Value;
                        ca.TryClear(now);
                    }
                }
            }
        }

        internal void Cache(ActionContext ac)
        {
            if (!ac.InCache && ac.Public == true && Cachie.IsCacheable(ac.Status))
            {
                Cachie ca = new Cachie(ac.Status, ac.Content, ac.MaxAge, Environment.TickCount);
                cachies.AddOrUpdate(ac.Uri, ca, (k, old) => ca.MergeWith(old));
                ac.InCache = true;
            }
        }

        internal bool TryGiveFromCache(ActionContext ac)
        {
            if (cachies.TryGetValue(ac.Uri, out var ca))
            {
                return ca.TryGive(ac, Environment.TickCount);
            }
            return false;
        }

        //
        // CLUSTER

        internal Client GetClient(string targetid)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                Client cli = clients[i];
                if (cli.Key.Equals(targetid)) return cli;
            }
            return null;
        }

        volatile bool stop;

        public void Stop()
        {
            stop = true;
        }

        public void Start()
        {
            if (events != null || clients != null)
            {
                EventQueue.Setup(this, clients);
            }

            // start the server
            //
            server.Start(this);

            OnStart();

            DBG(Key + " -> " + ServiceCtx.addrs[0] + " started");

            cleaner.Start();

            if (clients != null)
            {
                // Run in the scheduler thread to repeatedly check and initiate event polling activities.
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
            return (int) level >= ServiceCtx.logging;
        }

        static readonly string[] LVL = {"TRC: ", "DBG: ", "INF: ", "WAR: ", "ERR: ", "CRL: ", "NON: "};

        public void Log<T>(LogLevel level, EventId eid, T state, Exception exception, Func<T, Exception, string> formatter)
        {
            if (!IsEnabled(level))
            {
                return;
            }

            logWriter.Write(LVL[(int) level]);

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

        public void Dispose()
        {
            server.Dispose();

            logWriter.Flush();
            logWriter.Dispose();

            Console.Write(Key);
            Console.WriteLine(".");
        }
    }

    /// <summary>
    /// A microservice that implements authentication and authorization.
    /// </summary>
    /// <typeparam name="P">the principal type.</typeparam>
    public abstract class Service<P> : Service where P : class, IData, new()
    {
        protected Service(ServiceContext sc) : base(sc)
        {
        }

        /// <summary>
        /// To asynchronously process the request with authentication support.
        /// </summary>
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            ActionContext ac = NewMethod(context);
            HttpRequest req = ac.Request;
            string path = req.Path.Value;

            // authentication
            try
            {
                bool norm = true;
                if (this is IAuthenticateAsync aasync) norm = await aasync.AuthenticateAsync(ac, true);
                else if (this is IAuthenticate a) norm = a.Authenticate(ac, true);
                if (!norm)
                {
                    ac.Give(403); // forbidden
                    return;
                }
            }
            catch (Exception e)
            {
                DBG(e.Message);
            }

            // handling
            try
            {
                if ("/*".Equals(path)) // handle an event poll request
                {
                    Poll(ac);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Work work = Resolve(ref relative, ac);
                    if (work == null)
                    {
                        ac.Give(404); // not found
                        return;
                    }
                    await work.HandleAsync(relative, ac);
                }
            }
            catch (Exception e)
            {
                Catch(e, ac);
            }
            // sending
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

        private static ActionContext NewMethod(HttpContext context)
        {
            return (ActionContext) context;
        }

        internal void SetTokenCookie(ActionContext ac, P prin, short proj, int maxage = 0)
        {
            StringBuilder sb = new StringBuilder("Token=");
            string token = Encrypt(prin, proj);
            sb.Append(token);
            if (maxage > 0)
            {
                sb.Append("; Max-Age=").Append(maxage);
            }

            // obtain and add the domain attribute
            string host = ac.Header("Host");
            if (host != null)
            {
                int dot = host.LastIndexOf('.');
                if (dot > 0)
                {
                    dot = host.LastIndexOf('.', dot - 1);
                }
                if (dot > 0)
                {
                    string domain = host.Substring(dot);
                    sb.Append("; Domain=").Append(domain);
                }
            }

            sb.Append("; Path=/; HttpOnly");
            ac.SetHeader("Set-Cookie", sb.ToString());
        }

        public string Encrypt(P prin, short proj)
        {
            JsonContent cont = new JsonContent(true, 4096);
            cont.Put(null, prin, proj);
            byte[] bytebuf = cont.ByteBuffer;
            int count = cont.Size;

            int mask = (int) ServiceCtx.cipher;
            int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
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

        public P Decrypt(string token)
        {
            int mask = (int) ServiceCtx.cipher;
            int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
            int len = token.Length / 2;
            Str str = new Str(256);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform to byte
                int b = (byte) (Dv(token[p++]) << 4 | Dv(token[p++]));

                // masking
                str.Accept((byte) (b ^ masks[i % 4]));
            }

            // deserialize
            try
            {
                JObj jo = (JObj) new JsonParse(str.ToString()).Parse();
                P prin = new P();
                prin.Read(jo, -1);
                return prin;
            }
            catch
            {
                return null;
            }
        }

        // hexidecimal characters
        static readonly char[] HEX =
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