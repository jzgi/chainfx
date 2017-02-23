using System;
using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class Nodule : IRollable
    {
        protected static readonly AccessException AccessEx = new AccessException();

        // name as appeared in the uri path
        readonly string name;

        // access checks
        internal AccessAttribute[] roles;

        // filtering
        readonly FilterAttribute[] filters;

        internal UiAttribute ui;

        internal Nodule(string name, ICustomAttributeProvider attrs)
        {
            this.name = name;

            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // roles
            List<AccessAttribute> rolelst = null;
            foreach (var role in (AccessAttribute[])attrs.GetCustomAttributes(typeof(AccessAttribute), false))
            {
                if (rolelst == null)
                {
                    rolelst = new List<AccessAttribute>(8);
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

        public abstract Service Service { get; }

        public string Name => name;

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

        public bool IsModal => ui != null && ui.Modal > 0;

        public bool Check(ActionContext ac)
        {
            if (roles != null)
            {
                if (ac.Token == null) return false;

                // run checks
                for (int i = 0; i < roles.Length; i++)
                {
                    if (!roles[i].Check(ac))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void DoBefore(ActionContext ac)
        {
            if (filters == null) return;

            for (int i = 0; i < filters.Length; i++)
            {
                filters[i].Before(ac);
            }
        }

        internal void DoAfter(ActionContext ac)
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