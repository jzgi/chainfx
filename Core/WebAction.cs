using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor of an action method.
    /// </summary>
    public class WebAction : IKeyed
    {
        public WebSub Controller { get; }

        readonly Action<WebContext> doer;

        readonly Action<WebContext, string> doerVar;

        private IEnumerable<IfAttribute> ifs;

        public string Key { get; }

        public bool IsVar => doerVar != null;

        internal WebAction(WebSub controller, MethodInfo mi, bool isVar)
        {
            Controller = controller;
            // NOTE: strict method name as key here to avoid the default base url trap
            Key = mi.Name;
            if (!isVar)
            {
                doer = (Action<WebContext>)mi.CreateDelegate(typeof(Action<WebContext>), controller);
            }
            else
            {
                doerVar = (Action<WebContext, string>)mi.CreateDelegate(typeof(Action<WebContext, string>), controller);
            }
            ifs = mi.GetCustomAttributes<IfAttribute>();
        }

        internal bool Do(WebContext wc)
        {
            // check
            foreach (var @if in ifs)
            {
                if (!@if.Check(wc)) return false;
            }
            // invoke the action method
            doer(wc);
            return true;
        }

        internal bool Do(WebContext wc, string var)
        {
            // check
            foreach (var @if in ifs)
            {
                if (!@if.Check(wc, var)) return false;
            }
            // invoke the action method
            doerVar(wc, var);
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}