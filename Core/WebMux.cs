using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// <summary>
    /// A multiplexer controller that handles requests targeting variable-keys. 
    /// </summary>
    ///
    public abstract class WebMux : WebWork, IParent
    {
        // child controls
        private Roll<WebWork> children;

        protected WebMux(WebNodeContext wnc) : base(wnc) { }

        public Roll<WebWork> Children => children;

        public D AddChild<D>(string key, object state = null) where D : WebWork
        {
            if (children == null)
            {
                children = new Roll<WebWork>(16);
            }
            // create instance by reflection
            Type typ = typeof(D);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebNodeContext) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebNodeContext arg = new WebNodeContext
            {
                key = key,
                State = state,
                Parent = this,
                HasVar = true,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            // call the initialization and add
            D child = (D)ci.Invoke(new object[] { arg });
            children.Add(child);

            return child;
        }

        internal override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                DoRsc(relative, wc);
            }
            else // dispatch to child control
            {
                string dir = relative.Substring(0, slash);
                WebWork child;
                if (children != null && children.TryGet(relative, out child))
                {
                    child.Handle(relative.Substring(slash), wc);
                }
            }
        }

    }
    
}