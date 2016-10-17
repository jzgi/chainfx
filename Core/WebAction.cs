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

        readonly Action<WebContext, string> doer2;

        readonly IfAttribute[] ifs;

        public string Key { get; }

        public bool IsVar => doer2 != null;

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
                doer2 = (Action<WebContext, string>)mi.CreateDelegate(typeof(Action<WebContext, string>), controller);
            }

            // if attributes
            List<IfAttribute> lst = null;
            foreach (var @if in mi.GetCustomAttributes<IfAttribute>())
            {
                if (lst == null) lst = new List<IfAttribute>(8);
                lst.Add(@if);
            }
            ifs = lst?.ToArray();

        }

        internal bool TryDo(WebContext wc)
        {
            // check ifs
            if (ifs != null)
            {
                for (int i = 0; i < ifs.Length; i++)
                {
                    if (!ifs[i].Check(wc)) return false;
                }
            }

            // invoke the action method
            wc.Action = this;
            doer(wc);
            return true;
        }

        internal bool TryDo(WebContext wc, string var)
        {
            // check ifs
            if (ifs != null)
            {
                for (int i = 0; i < ifs.Length; i++)
                {
                    if (!ifs[i].Check(wc, var)) return false;
                }
            }

            // invoke the action method
            doer2(wc, var);
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}