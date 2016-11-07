using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{

    ///
    /// <summary>
    /// The descriptor for an action method.
    /// </summary>
    ///
    public class WebAction : IKeyed
    {
        public WebDo Control { get; }

        readonly Action<WebContext, string> doer;

        readonly CheckAttribute[] checks;

        readonly bool bearer, digest;

        readonly ButtonAttribute button;

        public string Key { get; }

        internal WebAction(WebDo control, MethodInfo mi)
        {
            Control = control;
            Key = mi.Name; // NOTE: strict method name as key here to avoid the default base url trap
            doer = (Action<WebContext, string>)mi.CreateDelegate(typeof(Action<WebContext, string>), control);

            // prepare if attributes
            List<CheckAttribute> lst = null;
            foreach (var to in mi.GetCustomAttributes<CheckAttribute>())
            {
                if (lst == null) lst = new List<CheckAttribute>(8);
                lst.Add(to);
                if (to.IsBearer) bearer = true;
                else digest = true;
            }
            checks = lst?.ToArray();

            button = mi.GetCustomAttribute<ButtonAttribute>();
        }

        public bool IsGet => button == null ? false : button.IsGet;

        public string Icon => button?.Icon;

        public int Dialog => button == null ? 3 : button.Dialog;

        // for generating unique digest nonce
        const string PrivateKey = "3e43a7180";

        internal bool TryInvoke(WebContext wc, string subscpt)
        {
            // access check 
            if (bearer || digest)
            {
                if (wc.Principal == null)
                {
                    wc.StatusCode = 401; // unauthorized
                    // challenge with bearer and digest dual schemes
                    string[] chlg = null;
                    if (bearer) chlg = chlg.Add("Bearer");
                    if (digest) chlg = chlg.Add("Digest realm=\"\", nonce=\"" + StrUtility.MD5(wc.Connection.RemoteIpAddress.ToString() + ':' + Environment.TickCount + ':' + PrivateKey) + "\"");
                    wc.SetHeader("WWW-Authenticate", chlg);
                    return false;
                }

                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i].Test(wc))
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