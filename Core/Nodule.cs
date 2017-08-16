using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A certain node of resources along the URi path.
    /// </summary>
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
        readonly IBefore before;

        readonly IBeforeAsync beforeasync;

        // post- operation
        readonly IAfter after;

        readonly IAfterAsync afterasync;

        internal Nodule(string name, ICustomAttributeProvider attrprov)
        {
            this.name = name;
            this.upper = name.ToUpper();

            // either methodinfo or typeinfo
            if (attrprov == null)
            {
                attrprov = GetType().GetTypeInfo();
            }

            // ui
            var uis = (UiAttribute[]) attrprov.GetCustomAttributes(typeof(UiAttribute), false);
            if (uis.Length > 0)
            {
                ui = uis[0];
            }

            // authorize
            var auths = (AuthorizeAttribute[]) attrprov.GetCustomAttributes(typeof(AuthorizeAttribute), false);
            if (auths.Length > 0)
            {
                authorize = auths[0];
                authorize.Nodule = this;
            }

            // filters
            var attrs = attrprov.GetCustomAttributes(false);
            for (int i = 0; i < attrs.Length; i++)
            {
                var a = attrs[i];
                if (a is IBefore) before = (IBefore) a;
                if (a is IBeforeAsync) beforeasync = (IBeforeAsync) a;
                if (a is IAfter) after = (IAfter) a;
                if (a is IAfterAsync) afterasync = (IAfterAsync) a;
            }
        }

        public abstract Service Service { get; }

        public string Name => name;

        public UiAttribute Ui => ui;

        public AuthorizeAttribute Authorize => authorize;

        public IBefore Before => before;

        public IBeforeAsync BeforeAsync => beforeasync;

        public IAfter After => after;

        public IAfterAsync AfterAsync => afterasync;

        public string Label => ui?.Label ?? upper;

        public bool HasUi => ui != null;

        public bool HasAuthorize => authorize != null;

        public bool DoAuthorize(ActionContext ac)
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