using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class WebNodule
    {
        // access filtering routines
        readonly FilterAttribute[] filters;

        // access checking routines
        internal ToAttribute[] grants;

        internal WebNodule(ICustomAttributeProvider attrs)
        {
            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // filters
            List<FilterAttribute> fltlst = null;
            foreach (var flt in (FilterAttribute[])attrs.GetCustomAttributes(typeof(FilterAttribute), false))
            {
                if (fltlst == null)
                {
                    fltlst = new List<FilterAttribute>(8);
                }
                flt.Nodule = this;
                fltlst.Add(flt);
            }
            this.filters = fltlst?.ToArray();

            // grants
            List<ToAttribute> grtlst = null;
            foreach (var grt in (ToAttribute[])attrs.GetCustomAttributes(typeof(ToAttribute), false))
            {
                if (grtlst == null)
                {
                    grtlst = new List<ToAttribute>(8);
                }
                grt.Nodule = this;
                grtlst.Add(grt);
            }
            this.grants = grtlst?.ToArray();
        }

        public abstract WebService Service { get; }

        public bool HasGrant(Type granttype)
        {
            if (grants != null)
            {
                for (int i = 0; i < grants.Length; i++)
                {
                    if (grants[i].GetType() == granttype) return true;
                }
            }
            return false;
        }

        internal bool Check(WebActionContext ac, bool reply = true)
        {
            if (grants != null)
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
                for (int i = 0; i < grants.Length; i++)
                {
                    if (!grants[i].Check(ac))
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