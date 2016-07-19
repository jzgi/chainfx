using System;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// Represents a sub-controller that consists of a group of action methods.
    ///
    public abstract class WebSub : IMember
    {
        // the service this sub-controller belongs to
        readonly WebService _service;

        // the default action
        readonly WebAction _default;

        // the collection of actions declared by this sub-controller
        readonly Set<WebAction> _actions = new Set<WebAction>(32);

        // the argument makes state-passing more convenient
        protected WebSub(WebService service)
        {
            _service = service;

            Type type = GetType();

            // introspect action methods
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 1 && pis[0].ParameterType == typeof(WebContext))
                {
                    WebAction a = new WebAction(this, mi);
                    if (a.Key.Equals("Default"))
                    {
                        _default = a;
                    }
                    _actions.Add(a);
                }
            }
        }

        ///
        /// The service this sub-controller belongs to
        ///
        public virtual WebService Service => _service;

        ///
        /// The key by which this sub-controller is added to its parent
        ///
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


    ///
    /// Represents a multiplexed sub-controller that consists of a group of action methods.
    ///
    public abstract class WebSub<TZone> : IMember where TZone : IZone
    {
        // the multiplexer that this sub-controller is added to
        readonly WebMux<TZone> _mux;

        // the default action
        readonly WebAction<TZone> _default;

        // the collection of multiplexed actions declared by this sub-controller
        readonly Set<WebAction<TZone>> _actions = new Set<WebAction<TZone>>(32);

        // the argument makes state-passing more convenient
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
                    _actions.Add(a);
                }
            }
        }

        ///
        /// The key by which this sub-controller is added to its parent
        ///
        public string Key { get; internal set; }

        public Checker<TZone> Checker { get; internal set; }

        public WebAction<TZone> GetAction(string action)
        {
            return _actions[action];
        }

        public virtual void Handle(string relative, WebContext wc)
        {
            WebAction<TZone> a;
            if (_actions.TryGet(relative, out a))
            {
                a.Do(wc, (TZone) (wc.Zone));
            }
            else
            {
                // send not found
            }
        }

        public abstract void Default(WebContext wc, TZone zone);
    }
}