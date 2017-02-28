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
    /// A service work implements a microservice.
    ///
    public abstract class Service : Folder, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {

        internal readonly string shard;

        internal readonly string[] addresses;

        internal readonly Db db;

        internal readonly Auth auth;

        internal readonly int logging;

        // the service instance id
        readonly string moniker;

        // the embedded server
        readonly KestrelServer server;

        // event consumption
        readonly Roll<EventInfo> events;

        // client connectivity to the related peers
        readonly Roll<Client> clients;

        EventU eventu;

        // event providing
        readonly Roll<EventQueue> queues;

        readonly ActionCache cache;

        Thread scheduler;

        Thread cleaner;

        protected Service(FolderContext fc) : base(fc)
        {

            JObj cfg = fc.Configuration;
            cfg.Get(nameof(shard), ref shard);
            cfg.Get(nameof(addresses), ref addresses);
            cfg.Get(nameof(db), ref db);
            cfg.Get(nameof(logging), ref logging);

            // adjust configuration
            fc.Service = this;

            moniker = (shard == null) ? fc.Name : fc.Name + "-" + shard;

            // setup logging 
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = fc.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false) { AutoFlush = true };

            // create kestrel instance
            KestrelServerOptions options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), ServerMain.Lifetime, factory);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (addresses == null)
            {
                throw new ServiceException("missing 'addresses'");
            }
            foreach (string a in addresses)
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
            JObj cluster = cfg[nameof(cluster)];
            if (cluster != null)
            {
                for (int i = 0; i < cluster.Count; i++)
                {
                    JMbr mbr = cluster[i];
                    if (clients == null)
                    {
                        clients = new Roll<Client>(cluster.Count * 2);
                    }
                    clients.Add(new Client(this, mbr.Name, (string)mbr));

                    if (queues == null)
                    {
                        queues = new Roll<EventQueue>(cluster.Count * 2);
                    }
                    queues.Add(new EventQueue(mbr.Name));
                }
            }

            // initialize response cache
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
        public string Moniker => moniker;

        public Roll<Client> Clients => clients;

        public Roll<EventInfo> Events => events;

        internal ActionCache Cache => cache;

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
                Service = this
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
                if ("/*".Equals(path)) // handle an event poll request
                {
                    if (queues == null)
                    {
                        ac.Reply(501); // not implemented
                    }
                    else
                    {
                        EventQueue eq;
                        string from = ac.Header("From");
                        if (from == null || (eq = queues[from]) == null)
                        {
                            ac.Reply(400); // bad request
                        }
                        else
                        {
                            eq.Poll(ac);
                        }
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
                else if (e is CheckException)
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
            DbContext dc = new DbContext(this);
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
            if (clients != null)
            {
                EventQueue.GlobalInit(this);
            }

            // start the server
            //
            server.Start(this);

            OnStart();

            // Debug.WriteLine(Name + " -> " + Context.addresses + " started");

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
                Thread.Sleep(5000);

                int tick = Environment.TickCount;
                for (int i = 0; i < Clients.Count; i++)
                {
                    Client client = Clients[i];
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
            return (int)level >= logging;
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

        volatile string connstr;

        public string ConnectionString
        {
            get
            {
                if (connstr == null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Host=").Append(db.host);
                    sb.Append(";Port=").Append(db.port);
                    sb.Append(";Database=").Append(db.database ?? Name);
                    sb.Append(";Username=").Append(db.username);
                    sb.Append(";Password=").Append(db.password);
                    sb.Append(";Read Buffer Size=").Append(1024 * 32);
                    sb.Append(";Write Buffer Size=").Append(1024 * 32);
                    sb.Append(";No Reset On Close=").Append(true);

                    connstr = sb.ToString();
                }
                return connstr;
            }
        }
        // hexidecimal characters
        protected static readonly char[] HEX =
        {
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'
        };

        public string Encrypt(IData token)
        {
            if (auth == null) return null;

            JsonContent cont = new JsonContent(true, true, 4096); // borrow
            cont.Put(null, token);
            byte[] bytebuf = cont.ByteBuffer;
            int count = cont.Size;

            int mask = auth.mask;
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

        public string Decrypt(string tokenstr)
        {
            if (auth == null) return null;

            int mask = auth.mask;
            int[] masks = { (mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff };
            int len = tokenstr.Length / 2;
            Text str = new Text(256);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // reordering

                // transform to byte
                int b = (byte)(Dv(tokenstr[p++]) << 4 | Dv(tokenstr[p++]));

                // masking
                str.Accept((byte)(b ^ masks[i % 4]));
            }
            return str.ToString();
        }


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

        public void SetBearerCookie(ActionContext ac, IData token)
        {
            StringBuilder sb = new StringBuilder("Bearer=");
            string tokenstr = Encrypt(token);
            sb.Append(tokenstr);
            sb.Append("; HttpOnly");
            if (auth.maxage != 0)
            {
                sb.Append("; Max-Age=").Append(auth.maxage);
            }
            // detect domain from the Host header
            string host = ac.Header("Host");
            if (!string.IsNullOrEmpty(host))
            {
                // if the last part is not numeric
                int lastdot = host.LastIndexOf('.');
                if (lastdot > -1 && !char.IsDigit(host[lastdot + 1])) // a domain name is given
                {
                    int dot = host.LastIndexOf('.', lastdot - 1);
                    if (dot != -1)
                    {
                        string domain = host.Substring(dot + 1);
                        sb.Append("; Domain=").Append(domain);
                    }
                }
            }
            // set header
            ac.SetHeader("Set-Cookie", sb.ToString());
        }

        ///
        /// The DB configuration embedded in a service context.
        ///
        public class Db : IData
        {
            // IP host or unix domain socket
            public string host;

            // IP port
            public int port;

            // default database name
            public string database;

            public string username;

            public string password;

            public void ReadData(IDataInput i, int proj = 0)
            {
                i.Get(nameof(host), ref host);
                i.Get(nameof(port), ref port);
                i.Get(nameof(database), ref database);
                i.Get(nameof(username), ref username);
                i.Get(nameof(password), ref password);
            }

            public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
            {
                o.Put(nameof(host), host);
                o.Put(nameof(port), port);
                o.Put(nameof(database), database);
                o.Put(nameof(username), username);
                o.Put(nameof(password), password);
            }
        }

        ///
        /// The web authetication configuration embedded in a service context.
        ///
        public class Auth : IData
        {
            // mask for encoding/decoding token
            public int mask;

            // repositioning factor for encoding/decoding token
            public int pose;

            // The number of seconds that a signon durates, or null if session-wide.
            public int maxage;

            // The service instance that does signon. Can be null if local
            public string moniker;

            public void ReadData(IDataInput i, int proj = 0)
            {
                i.Get(nameof(mask), ref mask);
                i.Get(nameof(pose), ref pose);
                i.Get(nameof(maxage), ref maxage);
                i.Get(nameof(moniker), ref moniker);
            }

            public void WriteData<R>(IDataOutput<R> o, int proj = 0) where R : IDataOutput<R>
            {
                o.Put(nameof(mask), mask);
                o.Put(nameof(pose), pose);
                o.Put(nameof(maxage), maxage);
                o.Put(nameof(moniker), moniker);
            }
        }
    }

    ///
    /// A microservice that implements authentication and authorization.
    ///
    public abstract class Service<TToken> : Service where TToken : IData, new()
    {
        protected Service(FolderContext fc) : base(fc) { }

        protected override void Authenticate(ActionContext ac)
        {
            string tokstr;
            string hv = ac.Header("Authorization");
            if (hv != null && hv.StartsWith("Bearer ")) // the Bearer scheme
            {
                tokstr = hv.Substring(7);
                string jsonstr = Decrypt(tokstr);
                ac.Token = JsonUtility.StringToObject<TToken>(jsonstr);
            }
            else if (ac.Cookies.TryGetValue("Bearer", out tokstr))
            {
                string jsonstr = Decrypt(tokstr);
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