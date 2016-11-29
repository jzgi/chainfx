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
        readonly WebDirectory dir;

        readonly string key;

        // one of the tow signatures
        readonly Delegate doer;

        // if the method has a subscript parameter
        readonly Type subtype;

        readonly CheckAttribute[] checks;

        readonly bool bearer, digest;

        readonly UiAttribute button;

        internal WebAction(WebDirectory dir, MethodInfo mi, Type subtyp)
        {
            this.dir = dir;
            this.key = mi.Name; // NOTE: strict method name as key here to avoid the default base url trap
            this.subtype = subtyp;

            // create delegate accordingly
            if (subtyp == null) doer = mi.CreateDelegate(typeof(Action<WebContext>), dir);
            else if (subtyp == typeof(string)) doer = mi.CreateDelegate(typeof(Action<WebContext, string>), dir);
            else if (subtyp == typeof(short)) doer = mi.CreateDelegate(typeof(Action<WebContext, short>), dir);
            else if (subtyp == typeof(int)) doer = mi.CreateDelegate(typeof(Action<WebContext, int>), dir);
            else if (subtyp == typeof(long)) doer = mi.CreateDelegate(typeof(Action<WebContext, long>), dir);
            else if (subtyp == typeof(DateTime)) doer = mi.CreateDelegate(typeof(Action<WebContext, DateTime>), dir);
            else throw new WebException(key + "(...) wrong subscript type");

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

            button = mi.GetCustomAttribute<UiAttribute>();
        }

        public WebDirectory Dir => dir;

        public string Key => key;

        public bool IsGet => button?.IsGet ?? false;

        public string Icon => button?.Icon;

        public int Dialog => button?.Dialog ?? 3;

        // for generating unique digest nonce
        const string PrivateKey = "3e43a7180";

        internal bool TryDo(WebContext wc, string subscpt = null)
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

            if (subtype == null) ((Action<WebContext>)doer)(wc);
            else if (subtype == typeof(string)) ((Action<WebContext, string>)doer)(wc, subscpt);
            else if (subtype == typeof(short)) ((Action<WebContext, short>)doer)(wc, subscpt.ToShort());
            else if (subtype == typeof(int)) ((Action<WebContext, int>)doer)(wc, subscpt.ToInt());
            else if (subtype == typeof(long)) ((Action<WebContext, long>)doer)(wc, subscpt.ToLong());
            else if (subtype == typeof(DateTime)) ((Action<WebContext, DateTime>)doer)(wc, subscpt.ToDateTime());

            wc.Action = null;
            return true;
        }

        public override string ToString()
        {
            return Key;
        }

        public static bool IsSubscriptType(Type t) =>
            t == typeof(string) || t == typeof(short) || t == typeof(int) || t == typeof(long) || t == typeof(DateTime);
    }
}