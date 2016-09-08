using System;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// Represents the root of a set of controllers, which may include sub controllers and/or a multiplexer hub controller.
    /// </summary>
    public abstract class WebRealm : WebSub, ICacheRealm
    {
        // the added sub controllers, if any
        private Set<WebSub> subs;

        // the attached multiplexer hub controller, if any
        private WebXHub xhub;

        protected WebRealm(WebServiceContext wsc) : base(wsc)
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
            Type type = typeof(TSub);
            ConstructorInfo ci = type.GetConstructor(new[] { typeof(WebServiceContext) });
            if (ci == null)
            {
                throw new WebException(type + ": the constructor not found (WebServiceContext)");
            }
            WebServiceContext wsc = new WebServiceContext
            {
                key = key,
                Parent = this,
                Service = Service,
                IsX = false
            };
            TSub sub = (TSub)ci.Invoke(new object[] { wsc });

            subs.Add(sub);

            //
            // check declared event handler methods

            return sub;
        }

        public THub SetXHub<THub>(bool auth) where THub : WebXHub
        {
            // create instance
            Type type = typeof(THub);
            ConstructorInfo ci = type.GetConstructor(new[] { typeof(WebServiceContext) });
            if (ci == null)
            {
                throw new WebException(type + ": the constructor not found (WebServiceContext)");
            }
            WebServiceContext wsc = new WebServiceContext
            {
                key = "X",
                Parent = this,
                Service = Service,
                IsX = true
            };
            THub hub = (THub)ci.Invoke(new object[] { wsc });

            // call the initialization and set
            xhub = hub;

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
                else if (xhub == null)
                {
                    wc.Response.StatusCode = 501; // Not Implemented
                }
                else
                {
                    wc.X = dir;
                    xhub.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }
    }
}