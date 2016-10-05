using System;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A section consists of sub controllers and/or variable-key hub controller.
    /// </summary>
    public abstract class WebHub : WebSub
    {
        // the added sub controllers, if any
        private Roll<WebSub> subs;

        // the attached variable-key multiplexer, if any
        private WebVarHub varhub;

        protected WebHub(WebInfo cfg) : base(cfg)
        {
        }

        public long LastModified { get; set; }

        public T AddSub<T>(string key, bool auth) where T : WebSub
        {
            if (subs == null)
            {
                subs = new Roll<WebSub>(16);
            }
            // create instance by reflection
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebInfo) });
            if (ci == null)
            {
                throw new WebServiceException(typ + ": the constructor with WebConfig");
            }
            WebConfig build = new WebConfig()
            {
                Key = key,
                Parent = this,
                Service = Service,
                IsVar = false
            };
            T sub = (T)ci.Invoke(new object[] { build });

            subs.Add(sub);

            return sub;
        }

        public T SetVarHub<T>(bool auth) where T : WebVarHub
        {
            // create instance
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebInfo) });
            if (ci == null)
            {
                throw new WebServiceException(typ + ": the constructor with WebConfig");
            }
            WebInfo build = new WebInfo
            {
                Key = "var",
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T hub = (T)ci.Invoke(new object[] { build });

            this.varhub = hub;

            return hub;
        }


        public override void Do(string rsc, WebContext wc)
        {
            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                base.Do(rsc, wc);
            }
            else // not local then sub & mux
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(dir, out sub))
                {
                    sub.Do(rsc.Substring(slash + 1), wc);
                }
                else if (varhub == null)
                {
                    wc.StatusCode = 501; // Not Implemented
                }
                else
                {
                    varhub.Do(rsc.Substring(slash + 1), wc, dir); // var = dir
                }
            }
        }
    }
}