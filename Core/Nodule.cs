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
        internal CheckAttribute[] checks;

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
            List<CheckAttribute> checklst = null;
            foreach (var check in (CheckAttribute[])attrs.GetCustomAttributes(typeof(CheckAttribute), false))
            {
                if (checklst == null)
                {
                    checklst = new List<CheckAttribute>(8);
                }
                check.Nodule = this;
                checklst.Add(check);
            }
            this.checks = checklst?.ToArray();

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

        public bool HasUi => ui != null;

        public bool IsModal => ui != null && ui.Modal > 0;

        public bool Check(ActionContext ac)
        {
            if (checks != null)
            {
                if (ac.Token == null) return false;

                // run checks
                for (int i = 0; i < checks.Length; i++)
                {
                    if (!checks[i].Check(ac))
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