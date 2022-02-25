using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SkyChain.Store;

namespace SkyChain.Web
{
    /// <summary>
    /// An embedded web service that wraps around the kestrel HTTP engine.
    /// </summary>
    public abstract class WebService : WebWork, IHttpApplication<HttpContext>, IServiceProvider
    {
        static readonly int ConcurrencyLevel = (int) (Math.Log(Environment.ProcessorCount, 2) + 1) * 2;

        //
        // http implementation

        string address;

        bool cache;

        string forward;

        short poll;

        // the embedded HTTP server
        readonly KestrelServer server;

        // as proxy
        WebClient client;

        // stacked entries to be polled
        ConcurrentDictionary<int, WebPollie> stacked;

        // the poller thread
        Thread poller;

        // shared cache of responses
        ConcurrentDictionary<string, WebCachie> shared;

        // the cache cleaner thread
        Thread cleaner;

        // networked sites
        ConcurrentDictionary<string, Peer> netted;


        protected WebService()
        {
            Service = this;
            Directory = "/";
            Pathing = "/";

            // create the embedded server instance
            var opts = new KestrelServerOptions
            {
                ApplicationServices = this
            };

            if (Application.Cert != null)
            {
                opts.ConfigureHttpsDefaults(https => https.ServerCertificate = Application.Cert);

                INF("cert is set");
            }

            server = new KestrelServer(Options.Create(opts), Application.TransportFactory, Application.Logger);
        }

        // IServiceProvider
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ILogger<KestrelServer>))
            {
                return Application.Logger;
            }
            if (serviceType == typeof(ILoggerFactory))
            {
                return Application.Logger;
            }
            return null;
        }

        public ConcurrentDictionary<string, Peer> Netted => netted;


        internal string Address
        {
            get => address;
            set // set server addr
            {
                address = value;
                var feat = server.Features.Get<IServerAddressesFeature>();
                feat.Addresses.Add(address);
            }
        }

        public bool Cache
        {
            get => cache;
            internal set
            {
                cache = value;
                if (cache)
                {
                    // create the response cache
                    shared = new ConcurrentDictionary<string, WebCachie>(ConcurrencyLevel, 1024);
                }
            }
        }

        public string Forward
        {
            get => forward;
            internal set
            {
                forward = value;
                if (forward != null)
                {
                    client = new WebClient(forward);
                }
            }
        }

        public short Poll
        {
            get => poll;
            internal set
            {
                poll = value;
                if (forward == null || poll > 0)
                {
                    stacked = new ConcurrentDictionary<int, WebPollie>(ConcurrencyLevel, 1024);
                }
            }
        }

        public bool IsProxy => forward != null;

        public bool IsProxyPlus => forward != null && poll > 0;

        internal async Task StartAsync(CancellationToken token)
        {
            await server.StartAsync(this, token);

            Console.WriteLine("[" + Name + "] started at " + address);

            if (forward != null)
            {
                Console.WriteLine("as a proxy to [" + forward + "]" + address);
            }

            // create & start the cleaner thread
            if (shared != null)
            {
                cleaner = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // cleaning cycle
                        Thread.Sleep(1000 * 12); // every 12 seconds 

                        // loop to clear or remove each expired items
                        int now = Environment.TickCount;
                        foreach (var ca in shared)
                        {
                            if (!ca.Value.TryClean(now))
                            {
                                shared.TryRemove(ca.Key, out _);
                            }
                        }
                    }
                });
                cleaner.Start();
            }

            // create & start the poller thread
            if (poll > 0)
            {
                poller = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // polling cycle
                        Thread.Sleep(1000 * poll);

                        // loop to clear or remove each expired items
                        int now = Environment.TickCount;
                        // client.GetArrayAsync<>()
                    }
                });
                poller.Start();
            }
        }

        internal async Task StopAsync(CancellationToken token)
        {
            await server.StopAsync(token);
        }


        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public async Task ProcessRequestAsync(HttpContext context)
        {
            var wc = (WebContext) context;

            var path = wc.Path;

            // determine it is static file
            try
            {
                if (path.EndsWith('*'))
                {
                    @enum(wc);
                }
                else
                {
                    var dot = path.LastIndexOf('.');
                    if (dot != -1) // file content from cache or file system
                    {
                        if (!TryGiveFromCache(wc))
                        {
                            GiveStaticFile(path, path.Substring(dot), wc);
                            TryAddForCache(wc);
                        }
                    }
                    else
                    {
                        if (await DoAuthenticate(wc))
                        {
                            await HandleAsync(path.Substring(1), wc);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (@catch != null) // If existing custom catch
                {
                    wc.Exception = e;
                    if (@catch.IsAsync) await @catch.DoAsync(wc, 0);
                    else @catch.Do(wc, 0);
                }
                else
                {
                    wc.Give(500, e.Message); // internal server error
                    Console.Write(e.StackTrace);
                }
            }
            finally
            {
                await wc.SendAsync();
            }
        }

        const int STATIC_MAX_AGE = 3600 * 48;

        public void GiveStaticFile(string filename, string ext, WebContext wc)
        {
            if (!StaticContent.TryGetType(ext, out var ctyp))
            {
                wc.Give(415, shared: true, maxage: STATIC_MAX_AGE); // unsupported media type
                return;
            }

            string path = Path.Join(Name, filename);
            if (!File.Exists(path))
            {
                wc.Give(404, shared: true, maxage: STATIC_MAX_AGE); // not found
                return;
            }

            // load file content
            var modified = File.GetLastWriteTime(path);
            byte[] bytes;
            bool gzip = false;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int len = (int) fs.Length;
                if (len > 2048)
                {
                    var ms = new MemoryStream(len);
                    using (var gzs = new GZipStream(ms, CompressionMode.Compress))
                    {
                        fs.CopyTo(gzs);
                    }

                    bytes = ms.ToArray();
                    gzip = true;
                }
                else
                {
                    bytes = new byte[len];
                    fs.Read(bytes, 0, len);
                }
            }

            var content = new StaticContent(bytes)
            {
                Key = filename,
                Type = ctyp,
                Adapted = modified,
                GZip = gzip
            };
            wc.Give(200, content, shared: true, maxage: STATIC_MAX_AGE);
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
            // close the context
            ((WebContext) context).Close();
        }

        public void Close()
        {
            server.Dispose();
        }

        //
        // RESPONSE CACHE

        internal void TryAddForCache(WebContext wc)
        {
            if (shared != null && wc.IsGet)
            {
                if (wc.Shared == true && wc.IsCacheable())
                {
                    var ca = new WebCachie(wc.Content)
                    {
                        Code = wc.StatusCode,
                        MaxAge = wc.MaxAge,
                        Tick = Environment.TickCount
                    };
                    shared.AddOrUpdate(wc.Uri, ca, (key, old) => old);
                }
            }
        }

        internal bool TryGiveFromCache(WebContext wc)
        {
            if (shared != null && wc.IsGet)
            {
                if (shared.TryGetValue(wc.Uri, out var ca))
                {
                    return ca.TryGiveBy(wc, Environment.TickCount);
                }
            }

            return false;
        }


        //
        // poll
        //

        /// <summary>
        /// Requested by polling from user-agent or a proxy.
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="key"></param>
        public void onpoll(WebContext wc, int key)
        {
            stacked.TryGetValue(key, out var v);

            string[] strs = null;
            var jc = new JsonContent(true, 16 * 1024);
            jc.Put(null, strs);

            wc.Give(200, jc, null, 0);
        }


        public void AddNotif(int key, string v)
        {
            var wp = stacked.GetOrAdd(key, (x) => new WebPollie());
        }

        public void DumpAllStacked()
        {
            // stacked.TryAdd();
            var jc = new JsonContent(true, 16);

            jc.OBJ_();

            foreach (var pair in stacked)
            {
                var key = pair.Key;
                var v = pair.Value;

                string[] arr = null;

                jc.Put(key.ToString(), arr);
            }

            jc._OBJ();
        }

        #region Web Actions

        /// <summary>
        /// To generate API reference documentation for this service.
        /// </summary>
        public void @ref(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.DIV_("uk-top-bar").H3(" API Reference")._DIV();
                h.DIV_("uk-top-placeholder")._DIV();

                Describe(h);
            });
        }

        /// <summary>
        /// To handle a URI ended with * for redirect.
        /// </summary>
        public virtual void @enum(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.DIV_("uk-top-bar").H3(" API Reference")._DIV();
                h.DIV_("uk-top-placeholder")._DIV();
            });
        }

        /// <summary>
        /// To handle a poll request and return sync data
        /// </summary>
        public virtual void @extern(WebContext wc)
        {
        }

        #endregion
    }
}