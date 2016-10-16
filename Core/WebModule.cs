using System;
using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    /// <summary>
    /// A module web directory controller that can contain sub- and varhub- controllers.
    /// </summary>
    public abstract class WebModule : WebSub, IParent
    {
        // the added sub controllers, if any
        private Roll<WebSub> subs;

        // the attached variable-key multiplexer, if any
        private WebVarHub varhub;

        protected WebModule(ISetting setg) : base(setg)
        {
        }

        public long LastModified { get; set; }

        public T AddSub<T>(string key, bool authen) where T : WebSub
        {
            if (subs == null)
            {
                subs = new Roll<WebSub>(16);
            }
            // create instance by reflection
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(ISetting) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebTie tie = new WebTie()
            {
                key = key,
                Authen = authen,
                Parent = this,
                Service = Service,
                IsVar = false
            };
            T sub = (T)ci.Invoke(new object[] { tie });

            subs.Add(sub);

            return sub;
        }

        public Roll<WebSub> Subs => subs;

        public WebVarHub VarHub => varhub;

        public T SetVarHub<T>(bool authen) where T : WebVarHub
        {
            // create instance
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(ISetting) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebTie tie = new WebTie
            {
                key = "var",
                Authen = authen,
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T hub = (T)ci.Invoke(new object[] { tie });

            this.varhub = hub;

            return hub;
        }

        protected internal override void Handle(string rsc, WebContext wc)
        {
            if (Authen && wc.Token == null)
            {
                wc.StatusCode = 401;
                wc.Response.Headers.Add("WWW-Authenticate", new StringValues("Bearer"));
                return;
            }

            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                base.Handle(rsc, wc);
            }
            else // not local then sub & mux
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(dir, out sub)) sub.Handle(rsc.Substring(slash + 1), wc);
                else if (varhub == null) wc.StatusCode = 501; // Not Implemented
                else varhub.Handle(rsc.Substring(slash + 1), wc, dir); // var = dir
            }
        }

    }
}