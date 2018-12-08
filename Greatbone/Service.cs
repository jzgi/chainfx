using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
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

namespace Greatbone
{
    /// <summary>
    /// A service is a work that implements HTTP endpoint.
    /// </summary>
    public abstract class Service : Work, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the identifier of this service instance
        readonly string name;

        // the embedded server
        readonly KestrelServer server;

        // configured connectors that connect to peer services
        readonly Map<string, Connector> connectors;

        List<Connector> polls;

        // the thread schedules and drives periodic jobs, such as event polling 
        Thread scheduler;

        // a secret string for trust of inter-service cumminication
        readonly string sign;

        // the response cache
        readonly ConcurrentDictionary<string, Resp> cache;

        // the response cache cleaner thread
        Thread cleaner;

        protected Service(ServiceConfig cfg) : base(cfg)
        {
            cfg.Service = this;

            name = cfg.Name;

            // init the file-based logger
            string file = cfg.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream stream = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(stream, Encoding.UTF8, 4096, false)
            {
                AutoFlush = true
            };

            // init the embedded server
            var options = new KestrelServerOptions();
            // configure https with server certificate
            string certfile = cfg.GetFilePath(ServiceUtility.CERT_PFX);
            if (File.Exists(certfile) && cfg.certpass != null)
            {
                try
                {
                    X509Certificate2 cert = new X509Certificate2(certfile, cfg.certpass, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                    options.ConfigureHttpsDefaults(x => x.ServerCertificate = cert);
                    INF("$cert.pfx loaded and configured");
                }
                catch (Exception e)
                {
                    WAR(e.Message);
                }
            }

            var loggerf = new LoggerFactory();
            loggerf.AddProvider(this);

            server = new KestrelServer(Options.Create(options), ServiceUtility.TransportFactory, loggerf);

            ICollection<string> addrs = server.Features.Get<IServerAddressesFeature>().Addresses;
            if (Addrs == null)
            {
                throw new ServiceException("missing 'addrs'");
            }
            foreach (string a in Addrs)
            {
                addrs.Add(a.Trim());
            }

            // initialize client connectors
            var refs = Refs;
            if (refs != null)
            {
                for (int i = 0; i < refs.Count; i++)
                {
                    var e = refs.EntryAt(i);
                    if (connectors == null)
                    {
                        connectors = new Map<string, Connector>(16);
                    }
                    connectors.Add(new Connector(e.Key, e.Value)
                    {
                        Service = this
                    });
                }
            }

            sign = Encrypt(cfg.cipher.ToString());

            // create the response cache
            if (cfg.cache)
            {
                int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
                cache = new ConcurrentDictionary<string, Resp>(factor * 4, 1024);
            }
        }

        ///
        /// Uniquely identify a service instance.
        ///
        public string Name => name;

        public ServiceConfig Config => (ServiceConfig) cfg;

        public string[] Addrs => ((ServiceConfig) cfg).addrs;

        public string Shard => ((ServiceConfig) cfg).shard;

        public string Descr => ((ServiceConfig) cfg).descr;

        public Db Db => ((ServiceConfig) cfg).db;

        public JObj Refs => ((ServiceConfig) cfg).refs;

        public int Logging => ((ServiceConfig) cfg).logging;

        public long Cipher => ((ServiceConfig) cfg).cipher;

        public string Sign => sign;

        public Map<string, Connector> Connectors => connectors;

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

        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public virtual async Task ProcessRequestAsync(HttpContext context)
        {
            WebContext wc = (WebContext) context;
            string path = wc.Path;
            try
            {
                await HandleAsync(path.Substring(1), wc);
            }
            catch (Exception ex)
            {
                if (Catch != null)
                {
                    wc.Exception = ex; // attatch exception to current context
                    if (Catch.IsAsync) await Catch.DoAsync(wc, 0);
                    else Catch.Do(wc, 0);
                }
                else
                {
                    wc.Give(500, ex.Message);
                }
            }
            // sending
            try
            {
                await wc.SendAsync();
            }
            catch (Exception e)
            {
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

            // close logger
            logWriter.Flush();
            logWriter.Dispose();
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
                        Thread.Sleep(30000); // every 30 seconds 
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
            if (polls != null)
            {
                // to repeatedly check and initiate event polling activities.
                scheduler = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // interval
                        Thread.Sleep(1000);

                        // a schedule cycle
                        int tick = Environment.TickCount;
                        for (int i = 0; i < polls.Count; i++)
                        {
                            var cli = polls[i];
                            cli.TryPollAsync(tick);
                        }
                    }
                });
                scheduler.Start();
            }
        }

        public void Schedule(string rname, Action<IPollContext> poller, short interval = 12)
        {
            if (connectors == null)
            {
                throw new ServiceException("webconfig missing refs");
            }
            // setup context for each designated client
            int match = 0;
            for (int i = 0; i < connectors.Count; i++)
            {
                var @ref = connectors.At(i);
                if (@ref.Key == rname)
                {
                    @ref.SetPoller(poller, interval);
                    if (polls == null) polls = new List<Connector>();
                    polls.Add(@ref);
                    match++;
                }
            }
            if (match == 0)
            {
                throw new ServiceException("webconfig refs missing " + rname);
            }
        }

        internal Connector GetRef(string key)
        {
            for (int i = 0; i < connectors.Count; i++)
            {
                Connector con = connectors.At(i);
                if (con.Key == key) return con;
            }
            return null;
        }

        public string ConnectionString => ((ServiceConfig) cfg).ConnectionString;

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

        public void Log<T>(LogLevel level, EventId eid, T state, Exception except, Func<T, Exception, string> formatter)
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
                var msg = formatter(state, except);
                logWriter.WriteLine(msg);
            }
            else // fixed format
            {
                logWriter.WriteLine(state.ToString());
                if (except != null)
                {
                    logWriter.WriteLine(except.StackTrace);
                }
            }
        }

        // end of a logger scope
        public void Dispose()
        {
        }

        //
        // RESPONSE CACHE

        internal void TryCacheUp(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (!wc.IsInCache && wc.Public == true && Resp.IsCacheable(wc.Code))
                {
                    var re = new Resp(wc.Code, wc.Content, wc.MaxAge, Environment.TickCount);
                    cache.AddOrUpdate(wc.Uri, re, (k, old) => re.MergeWith(old));
                    wc.IsInCache = true;
                }
            }
        }

        internal bool TryGiveFromCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (cache.TryGetValue(wc.Uri, out var re))
                {
                    return re.TryGive(wc, Environment.TickCount);
                }
            }

            return false;
        }

        public string Encrypt(string v)
        {
            byte[] bytebuf = Encoding.ASCII.GetBytes(v);
            int count = bytebuf.Length;
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
            return new string(charbuf, 0, charbuf.Length);
        }

        public string Encrypt<P>(P prin, byte proj) where P : IData
        {
            JsonContent cnt = new JsonContent(true, 4096);
            try
            {
                cnt.Put(null, prin, proj);
                byte[] bytebuf = cnt.ByteBuffer;
                int count = cnt.Size;

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
                return new string(charbuf, 0, charbuf.Length);
            }
            finally
            {
                // return pool
                BufferUtility.Return(cnt);
            }
        }

        public P Decrypt<P>(string token) where P : IData, new()
        {
            int mask = (int) Cipher;
            int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
            int len = token.Length / 2;
            var str = new Text(1024);
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
                JObj jo = (JObj) new JsonParser(str.ToString()).Parse();
                P prin = new P();
                prin.Read(jo, 0xff);
                return prin;
            }
            catch
            {
                return default;
            }
        }

        public string Decrypt(string v)
        {
            int mask = (int) Cipher;
            int[] masks = {(mask >> 24) & 0xff, (mask >> 16) & 0xff, (mask >> 8) & 0xff, mask & 0xff};
            int len = v.Length / 2;
            var str = new Text(1024);
            int p = 0;
            for (int i = 0; i < len; i++)
            {
                // TODO reordering

                // transform to byte
                int b = (byte) (Dv(v[p++]) << 4 | Dv(v[p++]));
                // masking
                str.Accept((byte) (b ^ masks[i % 4]));
            }
            return str.ToString();
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


        /// <summary>
        /// A prior response for caching that might be cleared but not removed, for better reusability. 
        /// </summary>
        public class Resp
        {
            // response status, 0 means cleared, otherwise one of the cacheable status
            short code;

            // can be set to null
            IContent content;

            // maxage in seconds
            int maxage;

            // time ticks when entered or cleared
            int stamp;

            int hits;

            internal Resp(short code, IContent content, int maxage, int stamp)
            {
                this.code = code;
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

            public bool IsCleared => code == 0;

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

                    if (code == 0) return pass < 900 * 1000; // 15 minutes

                    if (pass >= 0) // to clear this reply
                    {
                        code = 0; // set to cleared
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
                    if (code == 0)
                    {
                        return false;
                    }
                    short remain = (short) (((stamp + maxage * 1000) - now) / 1000); // remaining in seconds
                    if (remain > 0)
                    {
                        wc.IsInCache = true;
                        wc.Give(code, content, true, remain);
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
}