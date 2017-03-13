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

        readonly string upname;

        internal UiAttribute ui;

        // access check
        internal AuthorizeAttribute authorize;

        // operation(s)
        readonly WorkAttribute work;


        internal Nodule(string name, ICustomAttributeProvider attrs)
        {
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
            var works = (WorkAttribute[])attrs.GetCustomAttributes(typeof(WorkAttribute), false);
            if (works.Length > 0)
            {
                work = works[0];
                work.Nodule = this;
            }
        }

        public abstract Service Service { get; }

        public string Name => name;

        public UiAttribute Ui => ui;

        public AuthorizeAttribute Authorize => authorize;

        public WorkAttribute Work => work;

        public string Label => ui?.Label ?? upname;

        public bool HasUi => ui != null;

        public bool HasAuthorize => authorize != null;

        internal bool DoAuthorize(ActionContext ac)
        {
            if (authorize != null)
            {
                IData token = ac.Principal;
                if (token == null)
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