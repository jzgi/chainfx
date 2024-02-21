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

namespace ChainFX.Web
{
    /// <summary>
    /// An embedded web service that wraps around the kestrel HTTP engine.
    /// </summary>
    public abstract class WebService : WebWork, IHttpApplication<HttpContext>, IServiceProvider
    {
        static readonly int ConcurrencyLevel = (int)(Math.Log(Environment.ProcessorCount, 2) + 1) * 2;

        //
        // http implementation
        private string address;

        // the embedded HTTP engine
        private readonly KestrelServer server;


        // local shared cache or not
        private bool cache;


        private string proxy;

        // shared cache of previous responses
        private ConcurrentDictionary<string, WebStaticContent> shared;


        // the cache cleaner thread, can be null
        private Thread cleaner;


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

            if (Application.Certificate != null)
            {
                opts.ConfigureHttpsDefaults(https => https.ServerCertificate = Application.Certificate);
            }

            server = new KestrelServer(Options.Create(opts), Application.TransportFactory, Application.Logger);
        }

        internal void Init(string prop, JObj serviceConfig)
        {
            address = serviceConfig[nameof(address)];
            if (address == null)
            {
                throw new ApplicationException("missing 'url' in app.json web-" + prop);
            }
            var feat = server.Features.Get<IServerAddressesFeature>();
            feat.Addresses.Add(address);

            cache = serviceConfig[nameof(cache)];
            if (cache)
            {
                // create the response cache
                shared = new ConcurrentDictionary<string, WebStaticContent>(ConcurrencyLevel, 1024);
            }

            proxy = serviceConfig[nameof(proxy)];
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

        public string Address => address;

        public bool Cache => cache;

        public string Proxy => proxy;

        public string PublicUrl => proxy ?? address;


        protected internal async Task StartAsync(CancellationToken token)
        {
            await server.StartAsync(this, token);

            const int INTERVAL = 60 * 1000;

            // create & start the cleaner thread
            if (shared != null)
            {
                cleaner = new Thread(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        // cleaning cycle
                        Thread.Sleep(INTERVAL);

                        // loop to clear or remove each expired items
                        var nowtick = Environment.TickCount;

                        foreach (var (key, value) in shared)
                        {
                            if (value.IsStaleByNow(nowtick))
                            {
                                shared.TryRemove(key, out _);
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
        /// To asynchronously process the request. The C-procedural style avoids excessive recursive scheduling of tasks.
        /// </summary>
        public virtual async Task ProcessRequestAsync(HttpContext context)
        {
            var wc = (WebContext)context;

            var path = wc.Path;

            // determine it is static file
            try
            {
                // special handling for content enumeration
                if (path.EndsWith('*'))
                {
                    @enum(wc);
                    return;
                }

                // give file content from cache or file system
                var dot = path.LastIndexOf('.');
                if (dot != -1)
                {
                    if (!TryGiveFromCache(wc))
                    {
                        await GiveFileAsync(path, path.Substring(dot), wc);

                        TryAddToCache(wc);
                    }
                    return;
                }

                // do necessary authentication before entering the work
                if (wc.Principal == null && !await DoAuthenticateAsync(wc, false)) return;

                WebWork curwrk = this;
                var rsc = path.Substring(1);
                for (;;)
                {
                    if (!curwrk.DoAuthorize(wc, false)) throw new ForbiddenException(Name);

                    var varwrk = curwrk.VarWork;

                    int slash = rsc.IndexOf('/');
                    if (slash == -1) // is the targeted work
                    {
                        //
                        // resolve the resource
                        string name = rsc;
                        int subscpt = 0;
                        int dash = rsc.LastIndexOf('-');
                        if (dash != -1)
                        {
                            name = rsc.Substring(0, dash);
                            wc.Subscript = subscpt = rsc.Substring(dash + 1).ToInt();
                        }

                        var act = curwrk[name];
                        if (act == null)
                        {
                            // if name can be converted to a number then call default
                            var n = name.ToInt();
                            if (n > 0)
                            {
                                act = curwrk.Default;
                                wc.Subscript = subscpt = n;
                            }
                            else // name did not match any action
                            {
                                wc.GiveMsg(404, "Action not found", shared: true, maxage: 3600);
                                return;
                            }
                        }

                        //
                        // entering action level

                        wc.Action = act;

                        if (!act.DoAuthorize(wc, false)) throw new ForbiddenException(act.Name);

                        // try in the cache first
                        if (!Service.TryGiveFromCache(wc))
                        {
                            // invoke action method 
                            if (act.IsAsync) await act.DoAsync(wc, subscpt);
                            else act.Do(wc, subscpt);

                            Service.TryAddToCache(wc);
                        }

                        wc.Action = null;

                        return;
                    }
                    else // check sub works and var work
                    {
                        string key = rsc.Substring(0, slash);
                        var subwrk = curwrk.SubWorks?[key];
                        if (subwrk != null) // if child
                        {
                            // adjust rsc string
                            rsc = rsc.Substring(slash + 1);

                            // do necessary authentication before entering the work
                            if (wc.Principal == null && !await subwrk.DoAuthenticateAsync(wc, string.IsNullOrEmpty(rsc)))
                            {
                                return;
                            }

                            wc.AppendSeg(subwrk, key);

                            curwrk = subwrk;
                        }
                        else if (varwrk != null) // if variable-key subwork
                        {
                            // adjust rsc string
                            rsc = rsc.Substring(slash + 1);

                            // do necessary authentication before entering the work
                            if (wc.Principal == null && !await varwrk.DoAuthenticateAsync(wc, string.IsNullOrEmpty(rsc)))
                            {
                                return;
                            }

                            var prin = wc.Principal;
                            object accessor;
                            if (key.Length == 0)
                            {
                                if (prin == null) throw ForbiddenException.NoPrincipalError;

                                accessor = varwrk.GetAccessor(prin, null);
                                if (accessor == null)
                                {
                                    throw ForbiddenException.AccessorReq;
                                }
                            }
                            else
                            {
                                accessor = varwrk.GetAccessor(prin, key);
                            }
                            // append the segment
                            wc.AppendSeg(varwrk, key, accessor);

                            curwrk = varwrk;
                        }
                        else
                        {
                            wc.GiveMsg(404, "Work not found", shared: true, maxage: 3600);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (@catch != null) // If existing custom catch
                {
                    wc.Error = e;
                    if (@catch.IsAsync)
                    {
                        await @catch.DoAsync(wc, 0);
                    }
                    else
                        @catch.Do(wc, 0);
                }
                else
                {
                    wc.GiveMsg(500, e.Message, e.StackTrace); // internal server error
                }
            }
            finally
            {
                await wc.SendAsync();
            }
        }

        //
        // STATIC FILES
        //

        const int STATIC_FILE_MAXAGE = 3600 * 12;

        const int STATIC_FILE_GZIP_THRESHOLD = 1024 * 4;

        public async Task GiveFileAsync(string filename, string ext, WebContext wc)
        {
            if (!WebStaticContent.TryGetType(ext, out var ctype))
            {
                wc.Give(415, shared: true, maxage: STATIC_FILE_MAXAGE); // unsupported media type
                return;
            }

            string path = Path.Join(Folder, filename);
            if (!File.Exists(path))
            {
                wc.Give(404, shared: true, maxage: STATIC_FILE_MAXAGE); // not found
                return;
            }

            // load file content
            var modified = File.GetLastWriteTime(path);
            byte[] bytes;
            bool gzip = false;
            await using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                int len = (int)fs.Length;
                if (len > STATIC_FILE_GZIP_THRESHOLD)
                {
                    var ms = new MemoryStream(len);
                    await using (var gzs = new GZipStream(ms, CompressionMode.Compress))
                    {
                        await fs.CopyToAsync(gzs);
                    }
                    bytes = ms.ToArray();
                    gzip = true;
                }
                else
                {
                    bytes = new byte[len];
                    len = await fs.ReadAsync(bytes, 0, len);
                }
            }

            var staticrsc = new WebStaticContent(bytes, ctype)
            {
                Modified = modified,
                GZip = gzip
            };

            wc.Give(
                200,
                staticrsc,
                shared: true,
                maxage: STATIC_FILE_MAXAGE
            );
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
            ((WebContext)context).Close();
        }

        public void Close()
        {
            server.Dispose();
        }

        //
        // RESPONSE CACHE

        internal void TryAddToCache(WebContext wc)
        {
            if (shared != null && wc.Shared == true && wc.IsCacheable())
            {
                var ca = wc.ToCacheContent();

                shared.AddOrUpdate(wc.Uri, ca, (_, _) => ca);
            }
        }

        public bool TryGiveFromCache(WebContext wc)
        {
            if (shared != null && wc.IsGet)
            {
                if (shared.TryGetValue(wc.Uri, out var ca))
                {
                    return ca.TryGiveTo(wc, Environment.TickCount);
                }
            }

            return false;
        }


        //
        // poll
        //

        public void AddNotif(int key, string v)
        {
            // var wp = stacked.GetOrAdd(key, (x) => new EventBag());
        }


        #region Reserved Web Actions

        /// <summary>
        /// To generate API reference documentation for this service.
        /// </summary>
        public void @ref(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.DIV_("uk-top-bar").H3(" API Reference")._DIV();
                h.DIV_("uk-top-placeholder")._DIV();

                GenerateRestDoc(h);
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
        /// To process dara packs submitted by peer.
        /// </summary>
        public async Task @extern(WebContext wc)
        {
            var e = await wc.ReadAsync<Form>();
        }

        #endregion
    }
}