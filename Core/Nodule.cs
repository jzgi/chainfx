using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class Nodule : IRollable
    {
        public readonly AuthorizeException NoToken;
        public readonly AuthorizeException NoPermission;

        // name as appeared in the uri path
        readonly string name;

        readonly string upname;

        internal UiAttribute ui;

        // access check
        internal AuthorizeAttribute authorize;

        // operation(s)
        readonly FilterAttribute filter;


        internal Nodule(string name, ICustomAttributeProvider attrs)
        {
            NoToken = new AuthorizeException(true) { Nodoule = this };
            NoPermission = new AuthorizeException(false) { Nodoule = this };

            this.name = name;
            this.upname = name.ToUpper();

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

            // work
            var flts = (FilterAttribute[])attrs.GetCustomAttributes(typeof(FilterAttribute), false);
            if (flts.Length > 0)
            {
                filter = flts[0];
                filter.Nodule = this;
            }
        }

        public abstract Service Service { get; }

        public string Name => name;

        public UiAttribute Ui => ui;

        public AuthorizeAttribute Authorize => authorize;

        public FilterAttribute Filter => filter;

        public string Label => ui?.Label ?? upname;

        public bool HasUi => ui != null;

        public bool HasAuthorize => authorize != null;

        internal bool DoAuthorize(ActionContext ac)
        {
            if (authorize != null)
            {
                IData prin = ac.Principal;
                if (prin == null)
                {
                    return false;
                }
                return authorize.Check(ac);
            }
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }
}