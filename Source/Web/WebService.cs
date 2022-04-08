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
using static FabricQ.Web.Application;

namespace FabricQ.Web
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

        // the embedded HTTP engine
        readonly KestrelServer server;


        // local shared cache or not
        bool cache;

        // shared cache of previous responses
        ConcurrentDictionary<string, WebCacheEntry> shared;

        // the cache cleaner thread, can be null
        Thread cleaner;


        // outgoing web events 
        ConcurrentDictionary<int, WebEventLot> outbox;

        // interval for web event processing cycle, in seconds
        short cycle;

        // client connector to the origin service
        WebClient connector;

        // 
        ConcurrentDictionary<string, WebEventLot> inbox;


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

            if (Certificate != null)
            {
                opts.ConfigureHttpsDefaults(https => https.ServerCertificate = Certificate);

                INF("cert is set");
            }

            server = new KestrelServer(Options.Create(opts), TransportFactory, Logger);
        }

        // IServiceProvider
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ILogger<KestrelServer>))
            {
                return Logger;
            }
            if (serviceType == typeof(ILoggerFactory))
            {
                return Logger;
            }
            return null;
        }

        public string Address
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
                    shared = new ConcurrentDictionary<string, WebCacheEntry>(ConcurrencyLevel, 1024);
                }
            }
        }

        protected internal virtual async Task StartAsync(CancellationToken token)
        {
            await server.StartAsync(this, token);

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

            Console.WriteLine("[" + Name + "] started at " + address);
        }

        internal async Task StopAsync(CancellationToken token)
        {
            await server.StopAsync(token);
        }


        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public virtual async Task ProcessRequestAsync(HttpContext context)
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
                CType = ctyp,
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
                    var ca = new WebCacheEntry(wc.Content)
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


        public virtual async Task ProcessEventAsync(IotContext context)
        {
        }


        //
        // poll
        //

        ///
        /// Requested by polling from a proxy. respond with outgoing events
        /// 
        public void onswitch(WebContext wc, int key)
        {
            // stacked.TryGetValue(key, out var v);

            var t = wc.ReadAsync<Text>();
            
            
            
            // return outgoing

            string[] strs = null;
            var jc = new TextContent(true, 16 * 1024);
            // jc.Put(null, strs);

            wc.Give(200, jc, null, 0);
        }


        public void AddNotif(int key, string v)
        {
            // var wp = stacked.GetOrAdd(key, (x) => new EventBag());
        }

        public void DumpAllStacked()
        {
            // stacked.TryAdd();
            var jc = new JsonContent(true, 16);

            jc.OBJ_();

            // foreach (var pair in stacked)
            // {
            //     var key = pair.Key;
            //     var v = pair.Value;
            //
            //     string[] arr = null;
            //
            //     jc.Put(key.ToString(), arr);
            // }

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