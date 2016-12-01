using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// The descriptor for an action method.
    ///
    public class WebAction : IKeyed
    {
        readonly WebDirectory directory;

        readonly string key;

        readonly Action<WebExchange> doer;

        readonly CheckAttribute[] checks;

        // if requires auth through header or cookie
        readonly bool header, cookie;

        readonly UiAttribute ui;

        internal WebAction(WebDirectory dir, MethodInfo mi)
        {
            directory = dir;
            key = mi.Name;
            doer = (Action<WebExchange>)mi.CreateDelegate(typeof(Action<WebExchange>), dir);

            // prepare checks
            List<CheckAttribute> lst = null;
            foreach (var chk in mi.GetCustomAttributes<CheckAttribute>())
            {
                if (lst == null)
                {
                    lst = new List<CheckAttribute>(8);
                }
                lst.Add(chk);

                if (chk.IsCookie) header = true;
                else cookie = true;
            }
            checks = lst?.ToArray();

            ui = mi.GetCustomAttribute<UiAttribute>();
        }

        public WebDirectory Directory => directory;

        public string Key => key;

        public bool IsGet => ui?.IsGet ?? false;

        public string Icon => ui?.Icon;

        public int Dialog => ui?.Dialog ?? 3;

        // for generating unique digest nonce
        const string PrivateKey = "3e43a7180";

        internal bool TryDo(WebExchange we)
        {
            // access check 
            if (header && we.Principal == null)
            {
                we.StatusCode = 401; // unauthorized
                we.SetHeader("WWW-Authenticate", "Bearer");
                return false;
            }
            else if (cookie && we.Principal == null)
            {

            }

            for (int i = 0; i < checks.Length; i++)
            {
                if (!checks[i].Test(we))
                {
                    we.StatusCode = 403; // forbidden
                    return false;
                }
            }

            // invoke the method
            we.Action = this;
            doer(we);
            we.Action = null;
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}