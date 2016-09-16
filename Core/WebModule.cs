using System;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A independent set of controllers, including sub controllers and/or multiplexer hub controller.
    /// </summary>
    public abstract class WebModule : WebSub, ICacheRealm
    {
        // the added sub controllers, if any
        private Set<WebSub> subs;

        // the attached multiplexer controller, if any
        private WebVarHub hub;

        protected WebModule(WebConfig wsc) : base(wsc)
        {
        }

        public long LastModified { get; set; }

        public TSub AddSub<TSub>(string key, bool auth) where TSub : WebSub
        {
            if (subs == null)
            {
                subs = new Set<WebSub>(16);
            }
            // create instance by reflection
            Type typ = typeof(TSub);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebConfig) });
            if (ci == null)
            {
                throw new WebException(typ + ": the constructor not found (WebServiceContext)");
            }
            WebConfig cfg = new WebConfig
            {
                Key = key,
                Parent = this,
                Service = Service,
                IsVar = false
            };
            TSub sub = (TSub)ci.Invoke(new object[] { cfg });

            subs.Add(sub);

            return sub;
        }

        public THub SetVarHub<THub>(bool auth) where THub : WebVarHub
        {
            // create instance
            Type typ = typeof(THub);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebConfig) });
            if (ci == null)
            {
                throw new WebServiceException(typ + ": the constructor not found (WebServiceContext)");
            }
            WebConfig cfg = new WebConfig
            {
                Key = "X",
                Parent = this,
                Service = Service,
                IsVar = true
            };
            THub hub = (THub)ci.Invoke(new object[] { cfg });

            this.hub = hub;

            return hub;
        }


        public override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                base.Handle(relative, wc);
            }
            else // not local then sub & mux
            {
                string dir = relative.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(dir, out sub))
                {
                    sub.Handle(relative.Substring(slash + 1), wc);
                }
                else if (hub == null)
                {
                    wc.Response.StatusCode = 501; // Not Implemented
                }
                else
                {
                    wc.X = dir;
                    hub.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }
    }
}