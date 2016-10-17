using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>A variable-key multiplexer ontroller that is attached to a hub controller. </summary>
    ///
    public abstract class WebVarHub : WebSub, IParent
    {
        // the added sub controllers
        private Roll<WebSub> subs;

        protected WebVarHub(WebArg arg) : base(arg)
        {
        }

        public Roll<WebSub> Subs => subs;

        public T AddSub<T>(string key, bool auth) where T : WebSub
        {
            if (subs == null)
            {
                subs = new Roll<WebSub>(16);
            }
            // create instance by reflection
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = key,
                Auth = auth,
                Parent = this,
                IsVar = true,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            // call the initialization and add
            T sub = (T)ci.Invoke(new object[] { arg });
            subs.Add(sub);

            return sub;
        }

        internal override void Handle(string rsc, WebContext wc, string var)
        {
            if (!CheckAuth(wc)) return;

            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                wc.Control = this;
                Do(rsc, wc, var);
            }
            else // not local then sub
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(rsc, out sub))
                {
                    sub.Handle(rsc.Substring(slash), wc, var);
                }
            }
        }
    }
}