using System;
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

        private IfAttribute[] checkers;

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
        }

        internal void Do(WebContext wc)
        {
            doer(wc);
        }

        internal void Do(WebContext wc, string var)
        {
            doerVar(wc, var);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}