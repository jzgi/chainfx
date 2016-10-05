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

        protected WebVarHub(WebBuild cfg) : base(cfg)
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
            ConstructorInfo ci = type.GetConstructor(new[] { typeof(WebBuild) });
            if (ci == null)
            {
                throw new System.Net.WebException(type + ": the constructor not found (WebServiceContext)");
            }
            WebBuild cfg = new WebBuild
            {
                Key = key,
                Parent = this,
                Service = Service,
                IsVar = true
            };
            T sub = (T)ci.Invoke(new object[] { cfg });
            // call the initialization and add
            subs.Add(sub);

            return sub;
        }

        public override void Do(string rsc, WebContext wc, string var)
        {
            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                WebAction doer = GetAction(rsc);
                if (doer != null)
                {
                    doer.Do(wc, var);
                }
                else
                {
                    wc.StatusCode = 404;
                }
            }
            else // not local then sub
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs.TryGet(rsc, out sub))
                {
                    sub.Do(rsc.Substring(slash), wc);
                }
            }
        }
    }
}