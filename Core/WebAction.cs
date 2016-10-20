using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// The descriptor for an action handling method.
    /// </summary>
    ///
    public class WebAction : IKeyed
    {
        public WebControl Control { get; }

        readonly Action<WebContext, string> doer;

        readonly IfAttribute[] ifs;

        public string Key { get; }

        public bool IsVar => doer != null;

        internal WebAction(WebControl control, MethodInfo mi)
        {
            Control = control;
            Key = mi.Name; // NOTE: strict method name as key here to avoid the default base url trap
            doer = (Action<WebContext, string>)mi.CreateDelegate(typeof(Action<WebContext, string>), control);

            // prepare if attributes
            List<IfAttribute> lst = null;
            foreach (var @if in mi.GetCustomAttributes<IfAttribute>())
            {
                if (lst == null) lst = new List<IfAttribute>(8);
                lst.Add(@if);
            }
            ifs = lst?.ToArray();

        }

        internal bool TryDo(WebContext wc, string subscpt)
        {
            // check ifs
            if (ifs != null)
            {
                for (int i = 0; i < ifs.Length; i++)
                {
                    if (!ifs[i].Check(wc, subscpt)) return false;
                }
            }

            // invoke the action method
            doer(wc, subscpt);
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}