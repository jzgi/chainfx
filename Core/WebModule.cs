using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{

    /// <summary>
    /// A web module is a controller that can contain child- and multiplexer works.
    /// </summary>
    public abstract class WebModule : WebWork, IParent
    {
        const string VarKey = "-var-";

        // child controls, if any
        internal Roll<WebWork> children;

        // the attached multiplexer doer/controller, if any
        internal WebMux mux;

        protected WebModule(WebHierarchyContext whc) : base(whc)
        {
        }

        public W AddChild<W>(string key, object state = null) where W : WebWork
        {
            if (children == null)
            {
                children = new Roll<WebWork>(16);
            }
            // create instance by reflection
            Type typ = typeof(W);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebHierarchyContext) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebHierarchyContext arg = new WebHierarchyContext
            {
                key = key,
                State = state,
                Parent = this,
                IsVar = false,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            W work = (W)ci.Invoke(new object[] { arg });
            children.Add(work);

            return work;
        }

        public Roll<WebWork> Children => children;

        public WebMux Mux => mux;

        public M SetMux<M>(object state = null) where M : WebMux
        {
            // create instance
            Type typ = typeof(M);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebHierarchyContext) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebHierarchyContext arg = new WebHierarchyContext
            {
                key = VarKey,
                State = state,
                Parent = this,
                IsVar = true,
                Folder = (Parent == null) ? VarKey : Path.Combine(Parent.Folder, VarKey),
                Service = Service
            };
            M mux = (M)ci.Invoke(new object[] { arg });
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
                WebWork child;
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