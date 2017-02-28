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
        protected static readonly CheckException AccessEx = new CheckException();

        // name as appeared in the uri path
        readonly string name;

        // access checks
        internal CheckAttribute[] accesses;

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
            List<CheckAttribute> accesslst = null;
            foreach (var access in (CheckAttribute[])attrs.GetCustomAttributes(typeof(CheckAttribute), false))
            {
                if (accesslst == null)
                {
                    accesslst = new List<CheckAttribute>(8);
                }
                access.Nodule = this;
                accesslst.Add(access);
            }
            this.accesses = accesslst?.ToArray();

            // filters
            List<FilterAttribute> filterlst = null;
            foreach (var filter in (FilterAttribute[])attrs.GetCustomAttributes(typeof(FilterAttribute), false))
            {
                if (filterlst == null)
                {
                    filterlst = new List<FilterAttribute>(8);
                }
                filter.Nodule = this;
                filterlst.Add(filter);
            }
            this.filters = filterlst?.ToArray();

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

        public bool HasAccess(Type accesstyp)
        {
            if (accesses != null)
            {
                for (int i = 0; i < accesses.Length; i++)
                {
                    if (accesses[i].GetType() == accesstyp) return true;
                }
            }
            return false;
        }

        public bool HasUi => ui != null;

        public bool IsModal => ui != null && ui.Modal > 0;

        public bool CheckAccess(ActionContext ac)
        {
            if (accesses != null)
            {
                if (ac.Token == null) return false;

                // run checks
                for (int i = 0; i < accesses.Length; i++)
                {
                    if (!accesses[i].Check(ac))
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