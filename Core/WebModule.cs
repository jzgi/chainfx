using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A module web directory controller that can contain sub- and varhub- controllers.
    /// </summary>
    public abstract class WebModule : WebSub, IParent
    {
        // the added sub controllers, if any
        internal Roll<WebSub> subs;

        // the attached variable-key multiplexer, if any
        internal WebVarHub varhub;

        protected WebModule(WebArg arg) : base(arg)
        {
        }

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
                IsVar = false,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            T sub = (T)ci.Invoke(new object[] { arg });
            subs.Add(sub);

            return sub;
        }

        public Roll<WebSub> Subs => subs;

        public WebVarHub VarHub => varhub;

        public T SetVarHub<T>(bool auth) where T : WebVarHub
        {
            // create instance
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = "var",
                Auth = auth,
                Parent = this,
                IsVar = true,
                Folder = (Parent == null) ? "var" : Path.Combine(Parent.Folder, "var"),
                Service = Service
            };
            T hub = (T)ci.Invoke(new object[] { arg });
            this.varhub = hub;

            return hub;
        }

        internal override void Handle(string rsc, WebContext wc)
        {
            if (!CheckAuth(wc)) return;

            int slash = rsc.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                wc.Control = this;
                Do(rsc, wc);
            }
            else // not local then sub & mux
            {
                string dir = rsc.Substring(0, slash);
                WebSub sub;
                if (subs != null && subs.TryGet(dir, out sub)) // seek sub first
                {
                    sub.Handle(rsc.Substring(slash + 1), wc);
                }
                else if (varhub == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    varhub.Handle(rsc.Substring(slash + 1), wc, dir); // var = dir
                }
            }
        }

    }
}