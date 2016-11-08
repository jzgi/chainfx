using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{

    /// <summary>
    /// A module web directory controller that can contain child- and multiplexer controllers.
    /// </summary>
    public abstract class WebController : WebDoer, IParent
    {
        const string VarKey = "-var-";

        // child controls, if any
        internal Roll<WebDoer> children;

        // the attached multiplexer doer/controller, if any
        internal WebMuxer mux;

        protected WebController(WebArg arg) : base(arg)
        {
        }

        public D AddChild<D>(string key, object state = null) where D : WebDoer
        {
            if (children == null)
            {
                children = new Roll<WebDoer>(16);
            }
            // create instance by reflection
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = key,
                State = state,
                Parent = this,
                IsVar = false,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            D child = (D)ci.Invoke(new object[] { arg });
            children.Add(child);

            return child;
        }

        public Roll<WebDoer> Children => children;

        public WebMuxer Mux => mux;

        public D SetMuxer<D>(object state = null) where D : WebMuxer
        {
            // create instance
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = VarKey,
                State = state,
                Parent = this,
                IsVar = true,
                Folder = (Parent == null) ? VarKey : Path.Combine(Parent.Folder, VarKey),
                Service = Service
            };
            D mux = (D)ci.Invoke(new object[] { arg });
            this.mux = mux;

            return mux;
        }

        internal override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                DoRsc(relative, wc);
            }
            else // dispatch to child or multiplexer
            {
                string dir = relative.Substring(0, slash);
                WebDoer child;
                if (children != null && children.TryGet(dir, out child)) // seek sub first
                {
                    child.Handle(relative.Substring(slash + 1), wc);
                }
                else if (mux == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    wc.Var = dir;
                    mux.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }

    }

}