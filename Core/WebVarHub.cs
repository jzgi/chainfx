using System;
using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    /// <summary>A variable-key multiplexer ontroller that is attached to a hub controller. </summary>
    ///
    public abstract class WebVarHub : WebSub, IParent
    {
        // the added sub controllers
        private Roll<WebSub> subs;

        protected WebVarHub(ISetting setting) : base(setting)
        {
        }

        public Roll<WebSub> Subs => subs;

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
            WebTie tie = new WebTie
            {
                key = key,
                Authen = authen,
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T sub = (T)ci.Invoke(new object[] { tie });
            // call the initialization and add
            subs.Add(sub);

            return sub;
        }

        protected internal override void Handle(string rsc, WebContext wc, string var)
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
                WebAction a = GetAction(rsc);
                if (a != null) if (!a.Do(wc, var)) wc.StatusCode = 403; // forbidden
                    else wc.StatusCode = 404;
            }
            else // not local then sub
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs.TryGet(rsc, out sub)) sub.Handle(rsc.Substring(slash), wc);
            }
        }
    }
}