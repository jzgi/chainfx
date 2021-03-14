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

namespace SkyChain.Web
{
    /// <summary>
    /// An embedded web service that wraps around the kestrel HTTP engine.
    /// </summary>
    public abstract class WebService : WebWork, IHttpApplication<HttpContext>, IServiceProvider
    {
        //
        // http implementation

        string address;

        // the embedded HTTP server
        readonly KestrelServer server;

        // readonly LoggerFactory factory;

        // shared cache of recent responses
        readonly ConcurrentDictionary<string, Resp> shared;

        // the response cache cleaner thread
        Thread cleaner;

        protected WebService()
        {
            Service = this;
            Directory = "/";
            Pathing = "/";

            // factory = new LoggerFactory(new[] {ServerEnviron.logger});

            // create the response cache
            int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
            shared = new ConcurrentDictionary<string, Resp>(factor * 4, 1024);

            // create the embedded server instance
            var opts = new KestrelServerOptions
            {
                ApplicationServices = this
            };
            // var cert = ServerEnviron.BuildSelfSignedCertificate("jx.skyiah.com", "skyiah.com", "Skyiah", "Gs721004");
            // opts.ConfigureHttpsDefaults(https =>
            // {
            //     // https.ServerCertificate = cert; 
            //     
            // });

            server = new KestrelServer(Options.Create(opts), ServerEnviron.TransportFactory, ServerEnviron.logger);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ILogger<KestrelServer>))
            {
                return ServerEnviron.logger;
            }
            if (serviceType == typeof(ILoggerFactory))
            {
                return ServerEnviron.logger;
            }
            return null;
        }

        internal string Address
        {
            get => address;
            // set server addr
            set
            {
                address = value;
                server.Features.Get<IServerAddressesFeature>().Addresses.Add(address);
            }
        }


        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public async Task ProcessRequestAsync(HttpContext context)
        {
            var wc = (WebContext) context;

            string path = wc.Path;

            // determine it is static file
            try
            {
                int dot = path.LastIndexOf('.');
                if (dot != -1)
                {
                    // try to give file content from cache or file system
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

            var cnt = new StaticContent(bytes)
            {
                Key = filename,
                Type = ctyp,
                Modified = modified,
                GZip = gzip
            };
            wc.Give(200, cnt, shared: true, maxage: STATIC_MAX_AGE);
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

        /// <summary>
        /// To generate API reference documentation for this service.
        /// </summary>
        public void @ref(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.DIV_("uk-top-bar").T(Name).T(" API Reference")._DIV();
                h.DIV_("uk-top-placeholder")._DIV();

                Describe(h);
            });
        }

        internal async Task StartAsync(CancellationToken token)
        {
            await server.StartAsync(this, token);

            Console.WriteLine("[" + Name + "] started at " + address);

            // create and start the cleaner thread
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
                        foreach (var resp in shared)
                        {
                            if (!resp.Value.TryClean(now))
                            {
                                shared.TryRemove(resp.Key, out _);
                            }
                        }
                    }
                });
                cleaner.Start();
            }
        }

        internal async Task StopAsync(CancellationToken token)
        {
            await server.StopAsync(token);

            // close logger
            //            logWriter.Flush();
            //            logWriter.Dispose();
        }

        public void Close()
        {
            server.Dispose();
        }

        //
        // RESPONSE CACHE

        internal void TryAddForCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (!wc.IsInCache && wc.Shared == true && Resp.IsCacheable(wc.StatusCode))
                {
                    var resp = new Resp(wc.StatusCode, wc.Content, wc.MaxAge, Environment.TickCount);
                    shared.AddOrUpdate(wc.Uri, resp, (k, old) => resp.MergeWith(old));
                    wc.IsInCache = true;
                }
            }
        }

        internal bool TryGiveFromCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (shared.TryGetValue(wc.Uri, out var resp))
                {
                    return resp.TryGive(wc, Environment.TickCount);
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
            int code;

            // can be set to null
            IContent content;

            // maxage in seconds
            int maxage;

            // time ticks when entered or cleared
            int stamp;

            int hits;

            internal Resp(int code, IContent content, int maxage, int stamp)
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