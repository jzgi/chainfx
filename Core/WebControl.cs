using System.Collections.Generic;
using System.Reflection;

namespace Greatbone.Core
{
    public abstract class WebControl
    {
        readonly RoleAttribute[] roles;

        // if auth through header or cookie
        readonly bool header, cookie;

        readonly FilterAttribute[] filters;

        internal WebControl(ICustomAttributeProvider attrs)
        {
            // either methodinfo or typeinfo
            if (attrs == null)
            {
                attrs = GetType().GetTypeInfo();
            }

            // initialize checks
            List<RoleAttribute> roles = null;
            foreach (var role in (RoleAttribute[])attrs.GetCustomAttributes(typeof(RoleAttribute), false))
            {
                if (roles == null)
                {
                    roles = new List<RoleAttribute>(8);
                }
                role.Control = this;
                roles.Add(role);

                if (role.IsCookied) header = true;
                else cookie = true;
            }
            this.roles = roles?.ToArray();

            // initialize checks
            List<FilterAttribute> filters = null;
            foreach (var filter in (FilterAttribute[])attrs.GetCustomAttributes(typeof(FilterAttribute), false))
            {
                if (filters == null)
                {
                    filters = new List<FilterAttribute>(8);
                }
                filter.Control = this;
                filters.Add(filter);
            }
            this.filters = filters?.ToArray();
        }

        public abstract WebService Service { get; }

        public bool HasRole(RoleAttribute role)
        {
            if (roles != null)
            {
                for (int i = 0; i < roles.Length; i++)
                {
                    if (roles[i] == role) return true;
                }
            }
            return false;
        }

        internal bool Check(WebActionContext ac)
        {
            if (roles != null)
            {
                if (header && ac.Token == null)
                {
                    ac.Reply(401); // unauthorized
                    ac.SetHeader("WWW-Authenticate", "Bearer");
                    return false;
                }
                else if (cookie && ac.Token == null)
                {
                    string loc = Service.Auth.SignOn + "?orig=" + ac.Uri;
                    ac.Reply(303); // see other - redirect to signon url
                    ac.SetHeader("Location", loc);
                    return false;
                }

                for (int i = 0; i < roles.Length; i++)
                {
                    if (!roles[i].Check(ac))
                    {
                        ac.Reply(403); // forbidden
                        return false;
                    }
                }
            }
            return true;
        }

        internal void BeforeDo(WebActionContext ac)
        {
            if (filters == null) return;

            for (int i = 0; i < filters.Length; i++)
            {
                filters[i].Before(ac);
            }
        }

        internal void AfterDo(WebActionContext ac)
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