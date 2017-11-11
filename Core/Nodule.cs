using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A certain node of resources along the URi path.
    /// </summary>
    public abstract class Nodule : IRollable
    {
        // identifier as appeared in URI
        readonly string name;

        readonly bool cap;

        // ui label or upper key
        readonly string label;

        // user interface-related settings
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
            this.cap = !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);

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
                this.label = ui.Label ?? name?.ToUpper();
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
                var attr = attrs[i];
                if (attr is IBefore b) before = b;
                if (attr is IBeforeAsync basync) beforeasync = basync;
                if (attr is IAfter a) after = a;
                if (attr is IAfterAsync aasync) afterasync = aasync;
            }
        }

        public abstract Service Service { get; }

        public string Name => name;

        public bool IsCap => cap;

        public UiAttribute Ui => ui;

        public AuthorizeAttribute Authorize => authorize;

        public IBefore Before => before;

        public IBeforeAsync BeforeAsync => beforeasync;

        public IAfter After => after;

        public IAfterAsync AfterAsync => afterasync;

        public string Label => label;

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