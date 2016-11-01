using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{

    /// <summary>
    /// A module web directory controller that can contain child- and multiplexer controllers.
    /// </summary>
    public abstract class WebModule : WebControl, IParent
    {
        const string MultiplexKey = "*";

        // child controls, if any
        internal Roll<WebControl> children;

        // the attached multiplexer, if any
        internal WebMultiple multiple;

        protected WebModule(WebArg arg) : base(arg)
        {
        }

        public C AddChild<C>(string key, object state = null) where C : WebControl
        {
            if (children == null)
            {
                children = new Roll<WebControl>(16);
            }
            // create instance by reflection
            Type typ = typeof(C);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = key,
                State = state,
                Parent = this,
                IsMultiplex = false,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            C ctrl = (C)ci.Invoke(new object[] { arg });
            children.Add(ctrl);

            return ctrl;
        }

        public Roll<WebControl> Children => children;

        public WebMultiple Multiple => multiple;

        public C SetMultiple<C>(object state = null) where C : WebMultiple
        {
            // create instance
            Type typ = typeof(C);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = MultiplexKey,
                State = state,
                Parent = this,
                IsMultiplex = true,
                Folder = (Parent == null) ? MultiplexKey : Path.Combine(Parent.Folder, MultiplexKey),
                Service = Service
            };
            C mul = (C)ci.Invoke(new object[] { arg });
            this.multiple = mul;

            return mul;
        }

        internal override void Handle(string relative, WebContext wc)
        {
            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                wc.Control = this;
                Do(relative, wc);
                wc.Control = null;
            }
            else // dispatch to child or multiplexer
            {
                string dir = relative.Substring(0, slash);
                WebControl child;
                if (children != null && children.TryGet(dir, out child)) // seek sub first
                {
                    child.Handle(relative.Substring(slash + 1), wc);
                }
                else if (multiple == null)
                {
                    wc.StatusCode = 404; // not found
                }
                else
                {
                    wc.Super = dir;
                    multiple.Handle(relative.Substring(slash + 1), wc);
                }
            }
        }

    }

}