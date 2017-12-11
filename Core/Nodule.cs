using System.Reflection;

namespace Greatbone.Core
{
    /// <summary>
    /// A certain node of resources along the URi path.
    /// </summary>
    public abstract class Nodule : IMappable<string>
    {
        // identifier as appeared in URI
        readonly string name;

        // whether captital leading
        readonly bool capitalized;

        // name in lowercase
        readonly string lower;

        readonly string label;

        readonly string tip;

        readonly short group;

        // access check
        internal readonly AuthorizeAttribute authorize;

        // pre- operation
        readonly IBefore before;

        readonly IBeforeAsync beforeAsync;

        // post- operation
        readonly IAfter after;

        readonly IAfterAsync afterAsync;

        internal Nodule(string name, ICustomAttributeProvider attrp)
        {
            this.name = name ?? throw new ServiceException("null nodule name");
            this.capitalized = !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);
            this.lower = name.ToLower();

            // either methodinfo or typeinfo
            if (attrp == null)
            {
                attrp = GetType().GetTypeInfo();
            }

            // ui 
            var uis = (UiAttribute[]) attrp.GetCustomAttributes(typeof(UiAttribute), false);
            UiAttribute ui = uis.Length > 0 ? uis[0] : null;
            this.label = ui?.Label ?? name.ToUpper();
            this.tip = ui?.Tip ?? label;
            this.group = ui?.Group ?? 0;

            // authorize
            var auths = (AuthorizeAttribute[]) attrp.GetCustomAttributes(typeof(AuthorizeAttribute), false);
            if (auths.Length > 0)
            {
                authorize = auths[0];
                authorize.Nodule = this;
            }

            // filters
            var attrs = attrp.GetCustomAttributes(false);
            for (int i = 0; i < attrs.Length; i++)
            {
                var attr = attrs[i];
                if (attr is IBefore b) before = b;
                if (attr is IBeforeAsync basync) beforeAsync = basync;
                if (attr is IAfter a) after = a;
                if (attr is IAfterAsync aasync) afterAsync = aasync;
            }
        }

        public string Key => name;

        public bool IsCapitalized => capitalized;

        public string Lower => lower;

        public string Label => label;

        public string Tip => tip;

        public short Group => group;

        public AuthorizeAttribute Authorize => authorize;

        public IBefore Before => before;

        public IBeforeAsync BeforeAsync => beforeAsync;

        public IAfter After => after;

        public IAfterAsync AfterAsync => afterAsync;

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