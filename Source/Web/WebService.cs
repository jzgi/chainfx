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
using SkyChain.Source.Web;

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

        // shared cache of recent responses
        readonly ConcurrentDictionary<string, WebCache> shared;

        // the response cache cleaner thread
        Thread cleaner;

        protected WebService()
        {
            Service = this;
            Directory = "/";
            Pathing = "/";

            // create the response cache
            int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
            shared = new ConcurrentDictionary<string, WebCache>(factor * 4, 1024);

            // create the embedded server instance
            var opts = new KestrelServerOptions
            {
                ApplicationServices = this
            };

            if (ServerEnv.Cert != null)
            {
                opts.ConfigureHttpsDefaults(https => https.ServerCertificate = ServerEnv.Cert);

                INF("cert is set");
            }

            server = new KestrelServer(Options.Create(opts), ServerEnv.TransportFactory, ServerEnv.Logger);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(ILogger<KestrelServer>))
            {
                return ServerEnv.Logger;
            }
            if (serviceType == typeof(ILoggerFactory))
            {
                return ServerEnv.Logger;
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
                var feat = server.Features.Get<IServerAddressesFeature>();
                feat.Addresses.Add(address);
            }
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
                h.DIV_("uk-top-bar").H3(" API Reference")._DIV();
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
                if (!wc.IsInCache && wc.Shared == true && WebCache.IsCacheable(wc.StatusCode))
                {
                    var resp = new WebCache(wc.StatusCode, wc.Content, wc.MaxAge, Environment.TickCount);
                    shared.AddOrUpdate(wc.Uri, resp, (key, old) => old);
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
    }
}