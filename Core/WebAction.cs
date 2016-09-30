using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>The delegate of action methods.</summary>
    ///
    public delegate void WebDoer(WebContext wc);

    /// <summary>The delegate of mux action methods.</summary>
    ///
    public delegate void WebVarDoer(WebContext wc, string var);

    /// <summary>The descriptor of an action handling method.</summary>
    ///
    public class WebAction : IKeyed
    {
        public WebSub Controller { get; }

        readonly WebVarDoer vardoer;

        readonly WebDoer doer;

        private IfAttribute[] checkers;

        public string Key { get; }

        public bool IsVar => vardoer != null;

        internal WebAction(WebSub controller, MethodInfo mi, bool isVar)
        {
            Controller = controller;
            // NOTE: strict method name as key here to avoid the default base url trap
            Key = mi.Name;
            if (isVar)
            {
                vardoer = (WebVarDoer)mi.CreateDelegate(typeof(WebVarDoer), controller);
            }
            else
            {
                doer = (WebDoer)mi.CreateDelegate(typeof(WebDoer), controller);
            }
        }

        internal void Do(WebContext wc, string var)
        {
            vardoer(wc, var);
        }

        internal void Do(WebContext wc)
        {
            doer(wc);
        }

        public override string ToString()
        {
            return Key;
        }
    }
}