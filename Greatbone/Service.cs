using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

namespace Greatbone
{
    /// <summary>
    /// A service is a work that implements HTTP endpoint.
    /// </summary>
    public abstract class Service : Work, IHttpApplication<HttpContext>, ILoggerProvider, ILogger
    {
        // the identifier of this service instance
        readonly string key;

        // the embedded server
        readonly KestrelServer server;
        
        readonly Func<WebContext, bool> auth;
        readonly Func<WebContext, Task<bool>> authAsync;


        // configured clients that connect to peer services
        readonly Map<string, Client> clients;

        // the regular web pollers
        List<Poller> pollers;

        // the polling schesuler thread
        Thread scheduler;

        // the response cache
        readonly ConcurrentDictionary<string, Resp> cache;

        // the response cache cleaner thread
        Thread cleaner;

        protected Service(ServiceConfig cfg) : base(cfg)
        {
            cfg.Service = this;

            key = cfg.Name;

            // init the file-based logger
            string file = cfg.GetFilePath('$' + DateTime.Now.ToString("yyyyMM") + ".log");
            FileStream stream = new FileStream(file, FileMode.Append, FileAccess.Write);
            logWriter = new StreamWriter(stream, Encoding.UTF8, 4096, false)
            {
                AutoFlush = true
            };

            // init the embedded server
            var options = Options.Create(new KestrelServerOptions());
            var loggerf = new LoggerFactory();
            loggerf.AddProvider(this);
            server = new KestrelServer(options, ServiceUtility.TransportFactory, loggerf);

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
                    var e = refs.At(i);
                    if (clients == null)
                    {
                        clients = new Map<string, Client>(refs.Count * 2);
                    }
                    clients.Add(new Client(e.Key, e.Value));
                }
            }

            // create the response cache
            if (cfg.cache)
            {
                int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
                cache = new ConcurrentDictionary<string, Resp>(factor * 4, 1024);
            }
        }

        public ServiceConfig Config => (ServiceConfig) cfg;

        public string[] Addrs => ((ServiceConfig) cfg).addrs;

        public Db Db => ((ServiceConfig) cfg).db;

        public JObj Refs => ((ServiceConfig) cfg).refs;

        public int Logging => ((ServiceConfig) cfg).logging;

        public long Cipher => ((ServiceConfig) cfg).cipher;

        ///
        /// Uniquely identify a service instance.
        ///
        public override string Key => key;

        public Map<string, Client> Clients => clients;

        public List<Poller> Subscribes => pollers;

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
                    wc.Except = ex; // attatch exception to current context
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
            if (pollers != null)
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
                        for (int i = 0; i < pollers.Count; i++)
                        {
                            var poller = pollers[i];
                            poller.TryPoll(tick);
                        }
                    }
                });
                scheduler.Start();
            }
        }

        public void Poll(string svc, Action<WebContext> consumer)
        {
            if (clients == null)
            {
                throw new ServiceException("no client configured");
            }
            if (pollers == null)
            {
                pollers = new List<Poller>(4);
            }
            var poller = new Poller(svc, consumer);
            if (poller.Refs.Count == 0)
            {
                throw new ServiceException("no client configured for keySpec: " + svc);
            }
            pollers.Add(poller);
        }

        internal Client GetRef(string peerId)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                Client cli = clients[i];
                if (cli.Key == peerId) return cli;
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

        internal void TryCacheUp(WebContext wc)
        {
            if (wc.GET)
            {
                if (!wc.InCache && wc.Public == true && Resp.IsCacheable(wc.Status))
                {
                    var re = new Resp(wc.Status, wc.Content, wc.MaxAge, Environment.TickCount);
                    cache.AddOrUpdate(wc.Uri, re, (k, old) => re.MergeWith(old));
                    wc.InCache = true;
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

        public string Encrypt<P>(P prin, byte proj) where P : IData
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
                    if (status == 0)
                    {
                        return false;
                    }
                    short remain = (short) (((stamp + maxage * 1000) - now) / 1000); // remaining in seconds
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
}