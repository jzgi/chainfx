using System;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A section consists of sub controllers and/or variable-key hub controller.
    /// </summary>
    public abstract class WebHub : WebSub, ICacheRealm
    {
        // the added sub controllers, if any
        private Set<WebSub> subs;

        // the attached variable-key multiplexer, if any
        private WebVarSub varsub;

        protected WebHub(WebConfig cfg) : base(cfg)
        {
        }

        public long LastModified { get; set; }

        public T AddSub<T>(string key, bool auth) where T : WebSub
        {
            if (subs == null)
            {
                subs = new Set<WebSub>(16);
            }
            // create instance by reflection
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WebConfig)});
            if (ci == null)
            {
                throw new WebServiceException(typ + ": the constructor with WebConfig");
            }
            WebConfig cfg = new WebConfig
            {
                Key = key,
                Parent = this,
                Service = Service,
                IsVar = false
            };
            T sub = (T) ci.Invoke(new object[] {cfg});

            subs.Add(sub);

            return sub;
        }

        public T SetVarSub<T>(bool auth) where T : WebVarSub
        {
            // create instance
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] {typeof(WebConfig)});
            if (ci == null)
            {
                throw new WebServiceException(typ + ": the constructor with WebConfig");
            }
            WebConfig cfg = new WebConfig
            {
                Key = "var",
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T hub = (T) ci.Invoke(new object[] {cfg});

            this.varsub = hub;

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
                else if (varsub == null)
                {
                    wc.Response.StatusCode = 501; // Not Implemented
                }
                else
                {
                    wc.Var = dir;
                    varsub.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }
    }
}