using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// <summary>
    /// A multiplexer doer/controller handles requests that are targeted variable-keys. 
    /// </summary>
    ///
    public abstract class WebMuxDo : WebDo, IParent
    {
        // child controls
        private Roll<WebDo> children;

        protected WebMuxDo(WebArg arg) : base(arg) { }

        public Roll<WebDo> Children => children;

        public D AddChild<D>(string key, object state = null) where D : WebDo
        {
            if (children == null)
            {
                children = new Roll<WebDo>(16);
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
                IsVar = true,
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
                WebDo child;
                if (children != null && children.TryGet(relative, out child))
                {
                    child.Handle(relative.Substring(slash), wc);
                }
            }
        }

    }
    
}