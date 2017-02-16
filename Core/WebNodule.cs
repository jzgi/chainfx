using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class WebNodule : IRollable
    {
        // name as appeared in the uri path
        readonly string name;

        // access checks
        internal RoleAttribute[] roles;

        // filtering
        readonly FilterAttribute[] filters;

        internal UiAttribute ui;

        internal WebNodule(string name, ICustomAttributeProvider attrs)
        {
            this.name = name;

            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // roles
            List<RoleAttribute> rolelst = null;
            foreach (var role in (RoleAttribute[])attrs.GetCustomAttributes(typeof(RoleAttribute), false))
            {
                if (rolelst == null)
                {
                    rolelst = new List<RoleAttribute>(8);
                }
                role.Nodule = this;
                rolelst.Add(role);
            }
            this.roles = rolelst?.ToArray();

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

            // ui
            var uis = (UiAttribute[])attrs.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0)
            {
                ui = uis[0];
            }
        }

        public abstract WebService Service { get; }

        public string Name => name;

        public string Label => ui?.Label;

        public string Icon => ui?.Icon;

        public UiAttribute Ui => ui;

        public bool HasRole(Type roletyp)
        {
            if (roles != null)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i].GetType() == roletyp) return true;
                }
            }
            return false;
        }

        public bool HasUi => ui != null;

        public bool Check(WebActionContext ac, bool reply)
        {
            if (roles != null)
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
                        else // from non-browser
                        {
                            ac.SetHeader("WWW-Authenticate", "Bearer");
                            ac.Reply(401); // unauthorized
                        }
                    }
                    return false;
                }

                // run checks
                for (int i = 0; i < roles.Length; i++)
                {
                    if (!roles[i].Check(ac))
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

        public override string ToString()
        {
            return name;
        }
    }
}