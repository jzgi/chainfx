using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain level of resources management.
    ///
    public abstract class WebScope
    {
        // access checking routines
        readonly CheckAttribute[] checks;

        // access filtering routines
        readonly FilterAttribute[] filters;

        internal WebScope(ICustomAttributeProvider attrs)
        {
            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // initialize checks
            List<CheckAttribute> chklst = null;
            foreach (var chk in (CheckAttribute[])attrs.GetCustomAttributes(typeof(CheckAttribute), false))
            {
                if (chklst == null)
                {
                    chklst = new List<CheckAttribute>(8);
                }
                chk.Controller = this;
                chklst.Add(chk);
            }
            this.checks = chklst?.ToArray();

            // initialize checks
            List<FilterAttribute> fltlst = null;
            foreach (var flt in (FilterAttribute[])attrs.GetCustomAttributes(typeof(FilterAttribute), false))
            {
                if (fltlst == null)
                {
                    fltlst = new List<FilterAttribute>(8);
                }
                flt.Control = this;
                fltlst.Add(flt);
            }
            this.filters = fltlst?.ToArray();
        }

        public abstract WebService Service { get; }

        public bool HasCheck(Type checktyp)
        {
            if (checks != null)
            {
                for (int i = 0; i < checks.Length; i++)
                {
                    if (checks[i].GetType() == checktyp) return true;
                }
            }
            return false;
        }

        internal bool Allow(WebActionContext ac, bool reply = true)
        {
            if (checks != null)
            {
                if (ac.Token == null)
                {
                    if (reply)
                    {
                        if (ac.Header("Accept") != null) // if from browsing
                        {
                            string loc = Service.Authent?.SignOn + "?orig=" + ac.Uri;
                            ac.SetHeader("Location", loc);
                            ac.Reply(303); // see other - redirect to signon url
                        }
                        else // from non-browsing 
                        {
                            ac.Reply(401); // unauthorized
                            ac.SetHeader("WWW-Authenticate", "Bearer");
                        }
                    }
                    return false;
                }

                // run checks
                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i].Check(ac))
                    {
                        if (reply)
                        {
                            ac.Reply(403); // forbidden
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        internal void DoBefore(WebActionContext ac)
        {
            if (filters == null) return;

            for (int i = 0; i < filters.Length; i++)
            {
                filters[i].Before(ac);
            }
        }

        internal void DoAfter(WebActionContext ac)
        {
            if (filters == null) return;

            // execute in reversed order
            for (int i = filters.Length - 1; i >= 0; i--)
            {
                filters[i].After(ac);
            }
        }
    }
}