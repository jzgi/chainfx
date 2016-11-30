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

        // one of the tow signatures
        readonly Delegate doer;

        // if the method has a subscript parameter
        readonly Type subtype;

        readonly CheckAttribute[] checks;

        // if requires auth through header or cookie
        readonly bool header, cookie;

        readonly UiAttribute ui;

        internal WebAction(WebDirectory dir, MethodInfo mi, Type subtype)
        {
            this.directory = dir;
            this.key = mi.Name;
            this.subtype = subtype;

            // create the doer delegate
            if (subtype == null)
            {
                doer = mi.CreateDelegate(typeof(Action<WebContext>), dir);
            }
            else if (subtype == typeof(string))
            {
                doer = mi.CreateDelegate(typeof(Action<WebContext, string>), dir);
            }
            else if (subtype == typeof(short))
            {
                doer = mi.CreateDelegate(typeof(Action<WebContext, short>), dir);
            }
            else if (subtype == typeof(int))
            {
                doer = mi.CreateDelegate(typeof(Action<WebContext, int>), dir);
            }
            else if (subtype == typeof(long))
            {
                doer = mi.CreateDelegate(typeof(Action<WebContext, long>), dir);
            }
            else if (subtype == typeof(DateTime))
            {
                doer = mi.CreateDelegate(typeof(Action<WebContext, DateTime>), dir);
            }
            else throw new WebException(key + "(...) wrong subscript type");

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

        internal bool TryDo(WebContext wc, string subscpt = null)
        {
            // access check 
            if (header && wc.Principal == null)
            {
                wc.StatusCode = 401; // unauthorized
                wc.SetHeader("WWW-Authenticate", "Bearer");
                return false;
            }
            else if (cookie && wc.Principal == null)
            {

            }

            for (int i = 0; i < checks.Length; i++)
            {
                if (!checks[i].Test(wc))
                {
                    wc.StatusCode = 403; // forbidden
                    return false;
                }
            }

            // invoke the method
            wc.Action = this;
            if (subtype == null)
            {
                ((Action<WebContext>)doer)(wc);
            }
            else if (subtype == typeof(string))
            {
                ((Action<WebContext, string>)doer)(wc, subscpt);
            }
            else if (subtype == typeof(short))
            {
                ((Action<WebContext, short>)doer)(wc, subscpt.ToShort());
            }
            else if (subtype == typeof(int))
            {
                ((Action<WebContext, int>)doer)(wc, subscpt.ToInt());
            }
            else if (subtype == typeof(long))
            {
                ((Action<WebContext, long>)doer)(wc, subscpt.ToLong());
            }
            else if (subtype == typeof(DateTime))
            {
                ((Action<WebContext, DateTime>)doer)(wc, subscpt.ToDateTime());
            }
            wc.Action = null;
            return true;
        }

        public override string ToString()
        {
            return Key;
        }

        public static bool IsSubtype(Type typ) => typ == typeof(string) || typ == typeof(short) || typ == typeof(int) || typ == typeof(long) || typ == typeof(DateTime);
    }
}