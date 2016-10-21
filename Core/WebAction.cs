using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Primitives;

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
                if (wc.Token == null)
                {
                    wc.StatusCode = 401; // unauthorized
                    wc.Response.Headers.Add("WWW-Authenticate", new StringValues("Bearer"));
                    return false;
                }

                for (int i = 0; i < ifs.Length; i++)
                {
                    if (!ifs[i].Test(wc))
                    {
                        wc.StatusCode = 403; // forbidden
                        return false;
                    }
                }
            }

            // invoke the action method
            wc.Action = this;
            doer(wc, subscpt);
            wc.Action = null;
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}