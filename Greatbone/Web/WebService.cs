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

namespace Greatbone.Web
{
    /// <summary>
    /// An embedded web service that wraps around the kestrel HTTP engine.
    /// </summary>
    public abstract class WebService : WebWork, IHttpApplication<HttpContext>
    {
        //
        // http implementation

        string[] addrs;

        // shared cache or not
        bool cache = true;

        // cache of responses
        ConcurrentDictionary<string, Response> _cache;

        // the embedded HTTP server
        KestrelServer server;

        // the response cache cleaner thread
        Thread cleaner;

        protected WebService()
        {
            Service = this;
            Pathing = "/";
        }

        internal JObj Config
        {
            set
            {
                var cfg = value;

                // retrieve config settings
                cfg.Get(nameof(addrs), ref addrs);
                if (addrs == null)
                {
                    throw new FrameworkException("Missing 'addrs' configuration");
                }

                cfg.Get(nameof(cache), ref cache);
                if (cache)
                {
                    int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
                    // create the response cache
                    _cache = new ConcurrentDictionary<string, Response>(factor * 4, 1024);
                }

                // create the HTTP embedded server
                //
                var opts = new KestrelServerOptions();
                var logf = new LoggerFactory();
                logf.AddProvider(Framework.Logger);
                server = new KestrelServer(Options.Create(opts), Framework.TransportFactory, logf);

                var coll = server.Features.Get<IServerAddressesFeature>().Addresses;
                foreach (string a in addrs)
                {
                    coll.Add(a.Trim());
                }
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
                        TryToCache(wc);
                    }
                }
                else
                {
                    await HandleAsync(path.Substring(1), wc);
                }
            }
            catch (WebException e)
            {
                wc.Give(e.Code, e.Message);
            }
            catch (Exception e)
            {
                wc.Give(500, e.Message);
            }
            finally
            {
                await wc.SendAsync();
            }
        }

        public void GiveStaticFile(string filename, string ext, WebContext wc)
        {
            if (!StaticContent.TryGetType(ext, out var ctyp))
            {
                wc.Give(415); // unsupported media type
                return;
            }

            string path = Path.Join(Name, filename);
            if (!File.Exists(path))
            {
                wc.Give(404); // not found
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

            var cnt = new StaticContent(bytes, bytes.Length)
            {
                Key = filename,
                Type = ctyp,
                Modified = modified,
                GZip = gzip
            };
            wc.Give(200, cnt, shared: true, maxage: 60 * 15);
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

        internal async Task StartAsync(CancellationToken token)
        {
            await server.StartAsync(this, token);

            Console.WriteLine(Name + " started at " + addrs[0]);

            // create and start the cleaner thread
            if (_cache != null)
            {
                cleaner = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // cleaning cycle
                        Thread.Sleep(30000); // every 30 seconds 
                        // loop to clear or remove each expired items
                        int now = Environment.TickCount;
                        foreach (var re in _cache)
                        {
                            if (!re.Value.TryClean(now))
                            {
                                _cache.TryRemove(re.Key, out _);
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

        public void Dispose()
        {
            server.Dispose();
        }

        //
        // RESPONSE CACHE

        internal void TryToCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (!wc.IsInCache && wc.Shared == true && Response.IsCacheable(wc.StatusCode))
                {
                    var re = new Response(wc.StatusCode, wc.Content, wc.MaxAge, Environment.TickCount);
                    _cache.AddOrUpdate(wc.Uri, re, (k, old) => re.MergeWith(old));
                    wc.IsInCache = true;
                }
            }
        }

        internal bool TryGiveFromCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (_cache.TryGetValue(wc.Uri, out var resp))
                {
                    return resp.TryGive(wc, Environment.TickCount);
                }
            }

            return false;
        }


        /// <summary>
        /// A prior response for caching that might be cleared but not removed, for better reusability. 
        /// </summary>
        public class Response
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

            internal Response(int code, IContent content, int maxage, int stamp)
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

            internal Response MergeWith(Response old)
            {
                Interlocked.Add(ref hits, old.Hits);
                return this;
            }
        }
    }
}