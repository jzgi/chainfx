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
        // child controls, if any
        internal Roll<WebControl> controls;

        // the attached multiplexer, if any
        internal WebMultiple multiple;

        protected WebModule(WebArg arg) : base(arg)
        {
        }

        public T AddControl<T>(string key, bool auth) where T : WebControl
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
                Auth = auth,
                Parent = this,
                IsMulti = false,
                Folder = (Parent == null) ? key : Path.Combine(Parent.Folder, key),
                Service = Service
            };
            T ctrl = (T)ci.Invoke(new object[] { arg });
            controls.Add(ctrl);

            return ctrl;
        }

        public Roll<WebControl> Subs => controls;

        public WebMultiple VarHub => multiple;

        public T SetMultiple<T>(bool auth) where T : WebMultiple
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
                IsMulti = true,
                Folder = (Parent == null) ? "var" : Path.Combine(Parent.Folder, "var"),
                Service = Service
            };
            T mux = (T)ci.Invoke(new object[] { arg });
            this.multiple = mux;

            return mux;
        }

        internal override void Handle(string relative, WebContext wc)
        {
            if (!CheckAuth(wc)) return;

            int slash = relative.IndexOf('/');
            if (slash == -1) // handle it locally
            {
                wc.Control = this;
                Do(relative, wc);
            }
            else // not local then sub & mux
            {
                string dir = relative.Substring(0, slash);
                WebControl ctrl;
                if (controls != null && controls.TryGet(dir, out ctrl)) // seek sub first
                {
                    ctrl.Handle(relative.Substring(slash + 1), wc);
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