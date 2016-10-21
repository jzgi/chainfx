using System;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// <summary>
    /// A multiplexer that process many variable-keys. 
    /// </summary>
    ///
    public abstract class WebMultiple : WebControl, IParent
    {
        // child controls
        private Roll<WebControl> controls;

        protected WebMultiple(WebArg arg) : base(arg)
        {
        }

        public Roll<WebControl> Controls => controls;

        public T AddControl<T>(string key, object state = null) where T : WebControl
        {
            if (controls == null)
            {
                controls = new Roll<WebControl>(16);
            }
            // create instance by reflection
            Type typ = typeof(T);
            ConstructorInfo ci = typ.GetConstructor(new[] { typeof(WebArg) });
            if (ci == null) { throw new WebException(typ + ": the constructor with WebTie"); }
            WebArg arg = new WebArg
            {
                key = key,
                State = state,
                Parent = this,
                IsMultiplex = true,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            // call the initialization and add
            T sub = (T)ci.Invoke(new object[] { arg });
            controls.Add(sub);

            return sub;
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
            else // not local then child control
            {
                string dir = relative.Substring(0, slash);
                WebControl ctrl;
                if (controls != null && controls.TryGet(relative, out ctrl))
                {
                    ctrl.Handle(relative.Substring(slash), wc);
                }
            }
        }
    }
}