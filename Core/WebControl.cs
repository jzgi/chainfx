using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    public abstract class WebControl
    {
        readonly CheckAttribute[] checks;

        // if auth through header or cookie
        readonly bool header, cookie;

        readonly AlterAttribute[] alters;

        internal WebControl(ICustomAttributeProvider attrs)
        {
            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // initialize checks
            List<CheckAttribute> chks = null;
            foreach (var chk in (CheckAttribute[])attrs.GetCustomAttributes(typeof(CheckAttribute), false))
            {
                if (chks == null)
                {
                    chks = new List<CheckAttribute>(8);
                }
                chk.Control = this;
                chks.Add(chk);

                if (chk.IsCookied) header = true;
                else cookie = true;
            }
            checks = chks?.ToArray();

            // initialize checks
            List<AlterAttribute> alts = null;
            foreach (var alt in (AlterAttribute[])attrs.GetCustomAttributes(typeof(AlterAttribute), false))
            {
                if (alts == null)
                {
                    alts = new List<AlterAttribute>(8);
                }
                alt.Control = this;
                alts.Add(alt);
            }
            alters = alts?.ToArray();
        }

        public abstract WebService Service { get; }

        internal bool Check(WebActionContext ac)
        {
            if (checks != null)
            {
                if (header && ac.Token == null)
                {
                    ac.Status = 401; // unauthorized
                    ac.SetHeader("WWW-Authenticate", "Bearer");
                    return false;
                }
                else if (cookie && ac.Token == null)
                {
                    string loc = Service.Auth.SignOn + "?orig=" + ac.Uri;
                    ac.SetHeader("Location", loc);
                    ac.Status = 303; // see other - redirect to signon url
                    return false;
                }

                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i].Check(ac))
                    {
                        ac.Status = 403; // forbidden
                        return false;
                    }
                }
            }
            return true;
        }

        internal void BeforeDo(WebActionContext ac)
        {
            if (alters == null) return;

            for (int i = 0; i < alters.Length; i++)
            {
                alters[i].Before(ac);
            }
        }

        internal void AfterDo(WebActionContext ac)
        {
            if (alters == null) return;

            // execute in reversed order
            for (int i = alters.Length - 1; i >= 0; i--)
            {
                alters[i].After(ac);
            }
        }
    }
}