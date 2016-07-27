using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// Represents a (sub)controller that consists of a group of action methods, as well as a folder of static files.
    ///
    public abstract class WebSub : WebController
    {
        // the collection of actions declared by this sub-controller
        private readonly Set<WebAction> _actions = new Set<WebAction>(32);

        // the default action
        private readonly WebAction _defaction;


        public Checker Checker { get; internal set; }

        // the argument makes state-passing more convenient
        public WebSub(WebCreationContext wcc) : base(wcc)
        {
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
                        _defaction = a;
                    }
                    _actions.Add(a);
                }
            }
        }


        public WebAction GetAction(String action)
        {
            if (string.IsNullOrEmpty(action))
            {
                return _defaction;
            }
            return _actions[action];
        }

        public virtual void Handle(string relative, WebContext wc)
        {
            if (relative.IndexOf('.') != -1) // static handling
            {
                Static sta;
                if (Statics != null && Statics.TryGet(relative, out sta))
                {
                    wc.Response.SetContent(sta.ContentType, sta.Content, 0, sta.Length);
                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
            else
            { // action handling
                WebAction a = relative.Length == 0 ? _defaction : GetAction(relative);
                if (a == null)
                {
                    wc.Response.StatusCode = 404;
                }
                else
                {
                    a.Do(wc);
                }
            }
        }

        public virtual void Default(WebContext wc)
        {
            Static sta = IndexStatic;
            if (sta != null)
            {
                wc.Response.SetContent(sta.ContentType, sta.Content, 0, sta.Length);
            }
            else
            {
                // send not implemented
                wc.Response.StatusCode = 404;
            }
        }
    }


    ///
    /// Represents a multiplexed sub-controller that consists of a group of action methods.
    ///
    public abstract class WebSub<TZone> : WebController where TZone : IZone
    {
        // the collection of multiplexed actions declared by this sub-controller
        private readonly Set<WebAction<TZone>> _actions = new Set<WebAction<TZone>>(32);

        // the default action
        private readonly WebAction<TZone> _defaction;

        // the argument makes state-passing more convenient
        protected WebSub(WebCreationContext wcc) : base(wcc)
        {
            Type type = GetType();

            // introspect action methods
            foreach (MethodInfo mi in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                ParameterInfo[] pis = mi.GetParameters();
                if (pis.Length == 2 &&
                    pis[0].ParameterType == typeof(WebContext) &&
                    pis[0].ParameterType == typeof(TZone))
                {
                    WebAction<TZone> a = new WebAction<TZone>(this, mi);
                    if (a.Key.Equals("Default"))
                    {
                        _defaction = a;
                    }
                    _actions.Add(a);
                }
            }
        }


        public Checker<TZone> Checker { get; internal set; }

        public WebAction<TZone> GetAction(string action)
        {
            return _actions[action];
        }

        public virtual void Handle(string relative, WebContext wc)
        {
            if (relative.IndexOf('.') != -1) // static handling
            {
                Static sta;
                if (Statics != null && Statics.TryGet(relative, out sta))
                {
                    wc.Response.SetContent(sta.ContentType, sta.Content, 0, sta.Length);
                }
                else
                {
                    wc.Response.StatusCode = 404;
                }
            }
            else
            { // action handling
                WebAction<TZone> a = relative.Length == 0 ? _defaction : GetAction(relative);
                if (a == null)
                {
                    wc.Response.StatusCode = 404;
                }
                else
                {
                    a.Do(wc, (TZone)(wc.Zone));
                }
            }
        }

        public virtual void Default(WebContext wc, TZone zone)
        {
            Static sta = IndexStatic;
            if (sta != null)
            {
                wc.Response.SetContent(sta.ContentType, sta.Content, 0, sta.Length);
            }
            else
            {
                // send not implemented
                wc.Response.StatusCode = 404;
            }
        }
    }
}