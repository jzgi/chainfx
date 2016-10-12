using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>The delegate of  exchange doer  methods.</summary>
    ///
    public delegate void Doer(WebContext wc);

    /// <summary>The delegate of varied-key exchange doer methods.</summary>
    ///
    public delegate void VarDoer(WebContext wc, string var);

    /// <summary>The descriptor of an action handling method.</summary>
    ///
    public class WebAction : IKeyed
    {
        public WebSub Controller { get; }

        readonly VarDoer vardoer;

        readonly Doer doer;

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
                vardoer = (VarDoer)mi.CreateDelegate(typeof(VarDoer), controller);
            }
            else
            {
                doer = (Doer)mi.CreateDelegate(typeof(Doer), controller);
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