using System;
using System.Reflection;

// ReSharper disable All

namespace Greatbone.Core
{
    public abstract class WebSub : IMember
    {
        // the parent of this instance, if any
        readonly WebHub _hub;

        // the default do, if any
        readonly WebAction _default;

        Set<WebAction> _actions = new Set<WebAction>(32);

        ///
        /// make initialzation of controller more stablized
        protected WebSub(WebHub hub)
        {
            _hub = hub;

            Type type = GetType();

            // introspect action methods
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    WebAction atn = new WebAction(this, mi);
                    if (atn.Key.Equals("Default")) _default = atn;
                    if (_actions == null)
                    {
                        _actions = new Set<WebAction>(32);
                    }
                    _actions.Add(atn);
                }
            }
        }

        /// the key by which this service is added to its parent
        public string Key { get; internal set; }

        public Checker Checker { get; internal set; }

        public WebAction GetAction(String action)
        {
            if (string.IsNullOrEmpty(action))
            {
                return _default;
            }
            return _actions[action];
        }

        public virtual void Handle(string relative, WebContext wc)
        {
            WebAction a;
            if (_actions.TryGet(relative, out a))
            {
                a.Do(wc);
            }
            else
            {
                // send not found
            }
        }

        public abstract void Default(WebContext wc);
    }

    public abstract class WebSub<TZone> : IMember where TZone : IZone
    {
        // the parent multiplexer
        readonly WebMux<TZone> _mux;

        // the default action, if any
        readonly WebAction<TZone> _default;

        Set<WebAction<TZone>> _actions = new Set<WebAction<TZone>>(32);

        ///
        /// make initialzation of controller more stablized
        protected WebSub(WebMux<TZone> mux)
        {
            _mux = mux;

            Type type = GetType();

            // introspect action methods
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 2 && pis[0].ParameterType == typeof(WebContext) &&
                    pis[0].ParameterType == typeof(TZone))
                {
                    WebAction<TZone> a = new WebAction<TZone>(this, mi);
                    if (a.Key.Equals("Default")) _default = a;
                    if (_actions == null)
                    {
                        _actions = new Set<WebAction<TZone>>(32);
                    }
                    _actions.Add(a);
                }
            }
        }

        /// the key by which this service is added to its parent
        public string Key { get; internal set; }

        public Checker<TZone> Checker { get; internal set; }

        public WebAction<TZone> GetAction(String action)
        {
            return _actions[action];
        }

        public virtual void Handle(string relative, WebContext wc)
        {
            WebAction<TZone> a;
            if (_actions.TryGet(relative, out a))
            {
                a.Do(wc, (TZone) (wc.zone));
            }
            else
            {
                // send not found
            }
        }

        public abstract void Default(WebContext wc, TZone zone);
    }
}