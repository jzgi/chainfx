using System.Reflection;

namespace Greatbone.Core
{
    ///
    /// A certain node of resources along the URi path.
    ///
    public abstract class Nodule : IRollable
    {
        // name as appeared in the uri path
        readonly string name;

        // name in uppercase
        readonly string upper;

        // user interface settings
        internal UiAttribute ui;

        // access check
        internal AuthorizeAttribute authorize;

        // pre- operation
        readonly BeforeAttribute before;

        // post- operation
        readonly AfterAttribute after;


        internal Nodule(string name, ICustomAttributeProvider attrs)
        {
            this.name = name;
            this.upper = name.ToUpper();

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

            // before 
            var befs = (BeforeAttribute[])attrs.GetCustomAttributes(typeof(BeforeAttribute), false);
            if (befs.Length > 0)
            {
                before = befs[0];
                before.Nodule = this;
            }

            // after 
            var afts = (AfterAttribute[])attrs.GetCustomAttributes(typeof(AfterAttribute), false);
            if (afts.Length > 0)
            {
                after = afts[0];
                after.Nodule = this;
            }
        }

        public abstract Service Service { get; }

        public string Name => name;

        public UiAttribute Ui => ui;

        public AuthorizeAttribute Authorize => authorize;

        public BeforeAttribute Before => before;

        public AfterAttribute After => after;

        public string Label => ui?.Label ?? upper;

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