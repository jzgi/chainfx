using System;
using System.Net;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>A variable-key multiplexer ontroller that is attached to a hub controller. </summary>
    ///
    public abstract class WebVarHub : WebSub
    {
        // the added sub controllers
        private Roll<WebSub> subs;

        protected WebVarHub(WebConfig cfg) : base(cfg)
        {
        }

        public T AddSub<T>(string key, bool auth) where T : WebSub
        {
            if (subs == null)
            {
                subs = new Roll<WebSub>(16);
            }
            // create instance by reflection
            Type type = typeof(T);
            ConstructorInfo ci = type.GetConstructor(new[] {typeof(WebConfig)});
            if (ci == null)
            {
                throw new WebException(type + ": the constructor not found (WebServiceContext)");
            }
            WebConfig wsc = new WebConfig
            {
                Key = key,
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T sub = (T) ci.Invoke(new object[] {wsc});
            // call the initialization and add
            subs.Add(sub);

            return sub;
        }

        public override void Do(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // without a slash then handle it locally
            {
                WebDoer a = GetDoer(relative);
                a?.Do(wc, wc.Var);
            }
            else // not local then sub
            {
                string rsc = relative.Substring(0, slash);
                WebSub sub;
                if (subs.TryGet(rsc, out sub))
                {
                    sub.Do(rsc.Substring(slash), wc);
                }
            }
        }
    }
}