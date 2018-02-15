using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Greatbone.Core
{
    /// <summary>
    /// A service is a work that implements HTTP endpoint.
    /// </summary>
    public abstract class Service : Work, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the peer id of this service instance
        readonly string id;

        // the embedded server
        readonly KestrelServer server;

        // configured connectors to peers
        readonly Map<string, Connector> connectors;

        // descriptors of subscribed data flows
        Map<string, FlowDescript> flows;

        // the data flow polling schesuler thread
        Thread scheduler;

        // the response cache
        readonly ConcurrentDictionary<string, Resp> cache;

        // the response cache cleaner thread
        Thread cleaner;

        protected Service(ServiceConfig cfg) : base(cfg)
        {
            cfg.Service = this;

            id = (Shard == null) ? cfg.Name : cfg.Name + "-" + Shard;

            // setup file-based logger
            LoggerFactory factory = new LoggerFactory();
            factory.AddProvider(this);
            string file = cfg.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream fs = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(fs, Encoding.UTF8, 1024 * 4, false) {AutoFlush = true};

            // create the embedded kestrel instance
            KestrelServerOptions options = new KestrelServerOptions();
            server = new KestrelServer(Options.Create(options), ServiceUtility.Libuv, factory);
            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (Addrs == null)
            {
                throw new ServiceException("missing 'addrs'");
            }

            foreach (string a in Addrs)
            {
                addrs.Add(a.Trim());
            }

            // initialize connectors
            var cluster = Cluster;
            if (cluster != null)
            {
                for (int i = 0; i < cluster.Count; i++)
                {
                    var e = cluster.At(i);
                    if (connectors == null)
                    {
                        connectors = new Map<string, Connector>(cluster.Count * 2);
                    }

                    connectors.Add(new Connector(this, e.Key, e.Value));
                }
            }

            // create the response cache
            if (cfg.cache)
            {
                int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
                cache = new ConcurrentDictionary<string, Resp>(factor * 4, 1024);
            }
        }

        public string Shard => ((ServiceConfig) cfg).shard;

        public string[] Addrs => ((ServiceConfig) cfg).addrs;

        public Db Db => ((ServiceConfig) cfg).db;

        public Map<string, string> Cluster => ((ServiceConfig) cfg).cluster;

        public int Logging => ((ServiceConfig) cfg).logging;

        public long Cipher => ((ServiceConfig) cfg).cipher;

        ///
        /// Uniquely identify a service instance.
        ///
        public string Id => id;

        public Map<string, Connector> Connectors => connectors;

        public Map<string, FlowDescript> Flows => flows;

        public string Describe()
        {
            XmlContent cont = new XmlContent(false);
            Describe(cont);
            return cont.ToString();
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnStop()
        {
        }

        public virtual void Catch(Exception ex, WebContext wc)
        {
            WAR(ex.Message, ex);
            wc.Give(500, ex.Message);
        }

        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public virtual async Task ProcessRequestAsync(HttpContext context)
        {
            WebContext wc = (WebContext) context;
            HttpRequest req = wc.Request;
            string path = req.Path.Value;
            // handling
            try
            {
                if ("/*".Equals(path)) // handle an event poll request
                {
                    Poll(wc);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Work work = Resolve(ref relative, wc);
                    if (work == null)
                    {
                        wc.Give(404); // not found
                        return;
                    }

                    await work.HandleAsync(relative, wc);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, wc);
            }

            // sending
            try
            {
                await wc.SendAsync();
            }
            catch (Exception e)
            {
                ERR(e.Message, e);
                wc.Give(500, e.Message);
            }
        }

        ///
        /// Returns a framework custom context.
        ///
        public HttpContext CreateContext(IFeatureCollection features)
        {
            return new WebContext(features)
            {
                Service = this
            };
        }

        public void DisposeContext(HttpContext context, Exception excep)
        {
            // dispose the context
            ((WebContext) context).Dispose();
        }

        internal async Task StopAsync(CancellationToken token)
        {
            OnStop();

            await server.StopAsync(token);
        }

        internal async Task StartAsync(CancellationToken token)
        {
            // call custom construction
            OnStart();

            await server.StartAsync(this, token);

            Console.WriteLine(Key + " started at " + Addrs[0]);

            // create and start the cleaner thread
            if (cache != null)
            {
                cleaner = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // cleaning cycle
                        Thread.Sleep(1000 * 30); // 30 seconds 

                        // loop to clear or remove each expired items
                        int now = Environment.TickCount;
                        foreach (var re in cache)
                        {
                            if (!re.Value.TryClean(now))
                            {
                                cache.TryRemove(re.Key, out _);
                            }
                        }
                    }
                });
                cleaner.Start();
            }

            // create and start the scheduler thead
            if (connectors != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // interval
                        Thread.Sleep(5000);

                        // a schedule cycle
                        int tick = Environment.TickCount;
                        for (int i = 0; i < Connectors.Count; i++)
                        {
                            Connector connector = Connectors[i];
                            connector.TryPoll(tick);
                        }
                    }
                });
                scheduler.Start();
            }
        }

        //
        // CLUSTER

        internal void Poll(WebContext wc)
        {
            string from = wc.Header("From");
            long id = 0;
            using (var dc = NewDbContext())
            {
                dc.Query(dc.Sql("SELECT * FROM ").T(from).T(" WHERE pubid > @1 "), p => p.Set(id));
            }
        }

        public void Subscribe(string source, string dview, Func<object, int> datapack)
        {
            if (flows == null)
            {
                flows = new Map<string, FlowDescript>(8);
            }
        }

        internal Connector GetConnector(string peerid)
        {
            for (int i = 0; i < connectors.Count; i++)
            {
                Connector cli = connectors[i];
                if (cli.Key.Equals(peerid)) return cli;
            }

            return null;
        }

        public string ConnectionString => ((ServiceConfig) cfg).ConnectionString;

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
        // LOGGING

        // subworks are already there
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
            return (int) level >= Logging;
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
                var msg = formatter(state, exception);
                logWriter.WriteLine(msg);
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
            // close server
            server.Dispose();

            // close logger
            logWriter.Flush();
            logWriter.Dispose();
        }

        //
        // RESPONSE CACHE

        internal void TryCacheUp(WebContext ac)
        {
            if (ac.GET)
            {
                if (!ac.InCache && ac.Public == true && Resp.IsCacheable(ac.Status))
                {
                    var re = new Resp(ac.Status, ac.Content, ac.MaxAge, Environment.TickCount);
                    cache.AddOrUpdate(ac.Uri, re, (k, old) => re.MergeWith(old));
                    ac.InCache = true;
                }
            }
        }

        internal bool TryGiveFromCache(WebContext wc)
        {
            if (wc.GET)
            {
                if (cache.TryGetValue(wc.Uri, out var re))
                {
                    return re.TryGive(wc, Environment.TickCount);
                }
            }

            return false;
        }

        /// <summary>
        /// A prior response for caching that might be cleared but not removed, for better reusability. 
        /// </summary>
        public class Resp
        {
            // response status, 0 means cleared, otherwise one of the cacheable status
            int status;

            // can be set to null
            IContent content;

            // maxage in seconds
            int maxage;

            // time ticks when entered or cleared
            int stamp;

            int hits;

            internal Resp(int status, IContent content, int maxage, int stamp)
            {
                this.status = status;
                this.content = content;
                this.maxage = maxage;
                this.stamp = stamp;
            }

            /// <summary>
            ///  RFC 7231 cacheable status codes.
            /// </summary>
            public static bool IsCacheable(int code)
            {
                return code == 200 || code == 203 || code == 204 || code == 206 || code == 300 || code == 301 || code == 404 || code == 405 || code == 410 || code == 414 || code == 501;
            }

            public int Hits => hits;

            public bool IsCleared => status == 0;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="now"></param>
            /// <returns>false to indicate a removal of the entry</returns>
            internal bool TryClean(int now)
            {
                lock (this)
                {
                    int pass = now - (stamp + maxage * 1000);

                    if (status == 0) return pass < 900 * 1000; // 15 minutes

                    if (pass >= 0) // to clear this reply
                    {
                        status = 0; // set to cleared
                        content = null; // NOTE: the buffer won't return to the pool
                        maxage = 0;
                        stamp = now; // time being cleared
                    }

                    return true;
                }
            }

            internal bool TryGive(WebContext wc, int now)
            {
                lock (this)
                {
                    if (status == 0) return false;

                    int remain = ((stamp + maxage * 1000) - now) / 1000; // remaining in seconds
                    if (remain > 0)
                    {
                        wc.InCache = true;
                        wc.Give(status, content, true, remain);

                        Interlocked.Increment(ref hits);

                        return true;
                    }

                    return false;
                }
            }

            internal Resp MergeWith(Resp old)
            {
                Interlocked.Add(ref hits, old.Hits);
                return this;
            }
        }
    }

    /// <summary>
    /// A service that implements authentication and authorization.
    /// </summary>
    /// <typeparam name="P">the principal type.</typeparam>
    public abstract class Service<P> : Service where P : class, IData, new()
    {
        protected Service(ServiceConfig cfg) : base(cfg)
        {
        }

        /// <summary>
        /// To asynchronously process the request with authentication support.
        /// </summary>
        public override async Task ProcessRequestAsync(HttpContext context)
        {
            WebContext wc = (WebContext) context;
            HttpRequest req = wc.Request;
            string path = req.Path.Value;

            // authentication
            try
            {
                bool norm = true;
                if (this is IAuthenticateAsync aasync) norm = await aasync.AuthenticateAsync(wc, true);
                else if (this is IAuthenticate a) norm = a.Authenticate(wc, true);
                if (!norm)
                {
                    wc.Give(403); // forbidden
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
                    Poll(wc);
                }
                else // handle a regular request
                {
                    string relative = path.Substring(1);
                    Work work = Resolve(ref relative, wc);
                    if (work == null)
                    {
                        wc.Give(404); // not found
                        return;
                    }

                    await work.HandleAsync(relative, wc);
                }
            }
            catch (Exception ex)
            {
                Catch(ex, wc);
            }

            try // sending
            {
                await wc.SendAsync();
            }
            catch (Exception e)
            {
                ERR(e.Message, e);
                wc.Give(500, e.Message);
            }
        }

        internal void SetTokenCookie(WebContext wc, P prin, byte proj, int maxage = 0)
        {
            StringBuilder sb = new StringBuilder("Token=");
            string token = Encrypt(prin, proj);
            sb.Append(token);
            if (maxage > 0)
            {
                sb.Append("; Max-Age=").Append(maxage);
            }

            // obtain and add the domain attribute
            string host = wc.Header("Host");
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
            wc.SetHeader("Set-Cookie", sb.ToString());
        }

        public string Encrypt(P prin, byte proj)
        {
            JsonContent cont = new JsonContent(true, 4096);
            cont.Put(null, prin, proj);
            byte[] bytebuf = cont.ByteBuffer;
            int count = cont.Size;

            int mask = (int) Cipher;
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
            int mask = (int) Cipher;
            int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
            int len = token.Length / 2;
            var str = new Str(1024);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // TODO reordering

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
                prin.Read(jo, 0xff);
                return prin;
            }
            catch
            {
                return null;
            }
        }

        // hexidecimal characters
        readonly char[] HEX = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

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