using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Abstractions.Internal;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Greatbone.Web
{
    /// <summary>
    /// An embedded web server that wraps around the kestrel HTTP engine.
    /// </summary>
    public sealed class WebServer : IHttpApplication<HttpContext>
    {
        readonly string[] addrs;

        // the embedded server
        readonly KestrelServer server;

        // cache of responses
        readonly ConcurrentDictionary<string, Response> cache;

        // the response cache cleaner thread
        Thread cleaner;

        // dbset operation areas keyed by service id
        readonly ConcurrentDictionary<string, DbArea> areas;

        internal WebServer(AppConfig.Web cfg, ILoggerProvider logprov)
        {
            // init the embedded server
            var options = new KestrelServerOptions();
            ITransportFactory TransportFactory = new SocketTransportFactory(Options.Create(new SocketTransportOptions()), App.Lifetime, NullLoggerFactory.Instance);

            var logfac = new LoggerFactory();
            logfac.AddProvider(logprov);
            server = new KestrelServer(Options.Create(options), TransportFactory, logfac);

            ICollection<string> addrcoll = server.Features.Get<IServerAddressesFeature>().Addresses;
            this.addrs = cfg.addrs ?? throw new WebException("missing 'addrs'");
            foreach (string a in addrs)
            {
                addrcoll.Add(a.Trim());
            }

            int factor = (int) Math.Log(Environment.ProcessorCount, 2) + 1;
            // create the response cache
            if (cfg.cache)
            {
                cache = new ConcurrentDictionary<string, Response>(factor * 4, 1024);
            }
        }

        public WebWork RootWork { get; internal set; }


        /// <summary>
        /// To asynchronously process the request.
        /// </summary>
        public async Task ProcessRequestAsync(HttpContext context)
        {
            WebContext wc = (WebContext) context;

            string path = wc.Path;
            int dot = path.LastIndexOf('.');
            if (dot != -1) // the resource is a static file
            {
                if (!TryGiveFromCache(wc))
                {
                    GiveFile(path, path.Substring(dot), wc);
                    TryAddToCache(wc);
                }
            }
            else if (RootWork != null)
            {
                try
                {
                    await RootWork.HandleAsync(path.Substring(1), wc);
                }
                catch (Exception e)
                {
                    wc.Give(500, e.Message);
                }
            }
            else
            {
                wc.Give(404, "not found");
                return;
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

        public void GiveFile(string filename, string ext, WebContext wc)
        {
            if (!StaticContent.TryGetType(ext, out var ctyp))
            {
                wc.Give(415); // unsupported media type
                return;
            }

            string path = "web" + filename;
            if (!File.Exists(path))
            {
                wc.Give(404); // not found
                return;
            }

            // load file content
            DateTime modified = File.GetLastWriteTime(path);
            byte[] bytes;
            bool gzip = false;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int len = (int) fs.Length;
                if (len > 2048)
                {
                    var ms = new MemoryStream(len);
                    using (GZipStream gzs = new GZipStream(ms, CompressionMode.Compress))
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

            StaticContent cnt = new StaticContent(bytes, bytes.Length)
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

        internal async Task StopAsync(CancellationToken token)
        {
            await server.StopAsync(token);

            // close logger
//            logWriter.Flush();
//            logWriter.Dispose();
        }

        internal async Task StartAsync(CancellationToken token)
        {
            await server.StartAsync(this, token);

            Console.WriteLine(" started at " + addrs[0]);

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
        }

        //
        // RESPONSE CACHE

        internal void TryAddToCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (!wc.IsInCache && wc.Shared == true && Response.IsCacheable(wc.StatusCode))
                {
                    var re = new Response(wc.StatusCode, wc.Content, wc.MaxAge, Environment.TickCount);
                    cache.AddOrUpdate(wc.Uri, re, (k, old) => re.MergeWith(old));
                    wc.IsInCache = true;
                }
            }
        }

        internal bool TryGiveFromCache(WebContext wc)
        {
            if (wc.IsGet)
            {
                if (cache.TryGetValue(wc.Uri, out var resp))
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
            short code;

            // can be set to null
            IContent content;

            // maxage in seconds
            int maxage;

            // time ticks when entered or cleared
            int stamp;

            int hits;

            internal Response(short code, IContent content, int maxage, int stamp)
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