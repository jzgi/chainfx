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
        public WebDir Dir { get; }

        // one of the tow signatures
        readonly Delegate call;

        // if the method has a subscript parameter
        readonly Type subtype;

        readonly CheckAttribute[] checks;

        readonly bool bearer, digest;

        readonly ButtonAttribute button;

        public string Key { get; }

        internal WebAction(WebDir dir, MethodInfo mi, Type subtype)
        {
            Dir = dir;
            Key = mi.Name; // NOTE: strict method name as key here to avoid the default base url trap
            this.subtype = subtype;
            call =
                subtype == null ? mi.CreateDelegate(typeof(Action<WebContext>), dir) :
                subtype == typeof(string) ? mi.CreateDelegate(typeof(Action<WebContext, string>), dir) :
                subtype == typeof(short) ? mi.CreateDelegate(typeof(Action<short, WebContext>), dir) :
                subtype == typeof(int) ? mi.CreateDelegate(typeof(Action<int, WebContext>), dir) :
                subtype == typeof(long) ? mi.CreateDelegate(typeof(Action<long, WebContext>), dir) :
                subtype == typeof(DateTime) ? mi.CreateDelegate(typeof(Action<DateTime, WebContext>), dir) :
                null;

            // prepare checks
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

        public bool IsGet => button?.IsGet ?? false;

        public string Icon => button?.Icon;

        public int Dialog => button?.Dialog ?? 3;

        // for generating unique digest nonce
        const string PrivateKey = "3e43a7180";

        internal bool TryInvoke(WebContext wc, string subscpt = null)
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

            if (subtype == null) ((Action<WebContext>)call)(wc);
            else if (subtype == typeof(string)) ((Action<string, WebContext>)call)(subscpt, wc);
            else if (subtype == typeof(short)) ((Action<short, WebContext>)call)(subscpt.ToShort(), wc);
            else if (subtype == typeof(int)) ((Action<int, WebContext>)call)(subscpt.ToInt(), wc);
            else if (subtype == typeof(long)) ((Action<long, WebContext>)call)(subscpt.ToLong(), wc);
            else if (subtype == typeof(DateTime)) ((Action<DateTime, WebContext>)call)(subscpt.ToDateTime(), wc);

            wc.Action = null;
            return true;
        }

        public override string ToString()
        {
            return Key;
        }
    }
}