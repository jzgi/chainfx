using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    public abstract class WebAccess
    {
        WebService service;

        readonly CheckAttribute[] checks;

        // if auth through header or cookie
        readonly bool header, cookie;

        readonly AlterAttribute[] alters;

        readonly UiAttribute ui;

        internal WebAccess(ICustomAttributeProvider attrs)
        {
            // either methodinfo or typeinfo
            if (attrs == null) attrs = GetType().GetTypeInfo();

            // initialize checks
            List<CheckAttribute> chklst = null;
            foreach (var chk in (CheckAttribute[])attrs.GetCustomAttributes(typeof(CheckAttribute), false))
            {
                if (chklst == null)
                {
                    chklst = new List<CheckAttribute>(8);
                }
                chklst.Add(chk);

                if (chk.IsCookied) header = true;
                else cookie = true;
            }
            checks = chklst?.ToArray();

            // initialize checks
            List<AlterAttribute> altlst = null;
            foreach (var alt in (AlterAttribute[])attrs.GetCustomAttributes(typeof(AlterAttribute), false))
            {
                if (altlst == null)
                {
                    altlst = new List<AlterAttribute>(8);
                }
                altlst.Add(alt);
            }
            alters = altlst?.ToArray();

            // initialize ui
            var uis = (UiAttribute[])attrs.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0) ui = uis[0];
        }

        internal bool Check(WebActionContext ac)
        {
            if (checks != null)
            {
                if (header && ac.Principal == null)
                {
                    ac.StatusCode = 401; // unauthorized
                    ac.SetHeader("WWW-Authenticate", "Bearer");
                    return false;
                }
                else if (cookie && ac.Principal == null)
                {
                    string loc = service.SignOn + "?orig=" + ac.Uri;
                    ac.SetHeader("Location", loc);
                    ac.StatusCode = 303; // see other - redirect to signon url
                    return false;
                }

                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i].Check(ac))
                    {
                        ac.StatusCode = 403; // forbidden
                        return false;
                    }
                }
            }
            return true;
        }

        internal void Before(WebActionContext ac)
        {
            if (alters == null) return;

            for (int i = 0; i < alters.Length; i++)
            {
                alters[i].Before(ac);
            }
        }

        internal void After(WebActionContext ac)
        {
            if (alters == null) return;

            // execute in reversed order
            for (int i = alters.Length - 1; i <= 0; i--)
            {
                alters[i].After(ac);
            }
        }
    }
}