using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class Nodule : IRollable
    {
        protected static readonly AuthorizeException AuthorizeEx = new AuthorizeException();

        // name as appeared in the uri path
        readonly string name;

        internal UiAttribute ui;

        // access check
        internal AuthorizeAttribute authorize;

        // filtering
        readonly FilterAttribute[] filters;

        internal Nodule(string name, ICustomAttributeProvider attrs)
        {
            this.name = name;

            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // ui
            var uis = (UiAttribute[])attrs.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0)
            {
                ui = uis[0];
            }

            // authorize
            var auths = (AuthorizeAttribute[])attrs.GetCustomAttributes(typeof(AuthorizeAttribute), false);
            if (auths.Length > 0)
            {
                authorize = auths[0];
                authorize.Nodule = this;
            }

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
        }

        public abstract Service Service { get; }

        public string Name => name;

        public AuthorizeAttribute Authorize => authorize;

        public UiAttribute Ui => ui;

        public string Label => ui?.Label ?? name;

        public bool HasUi => ui != null;

        public bool HasAuthorize => authorize != null;

        public bool DoAuthorize(ActionContext ac)
        {
            if (authorize != null)
            {
                IData token = ac.Token;
                if (token == null)
                {
                    return false;
                }
                return authorize.Check(ac);
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