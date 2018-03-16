using System.Reflection;

namespace Greatbone
{
    /// <summary>
    /// A certain node of resources along URI path.
    /// </summary>
    public abstract class Nodule : IMappable<string>
    {
        // identifier as appeared in URI
        readonly string name;

        // whether captital leading
        readonly bool capital;

        // name in lowercase
        readonly string lower;

        readonly string label;

        readonly string tip;

        readonly byte flag;

        // access check
        internal readonly AuthorizeAttribute authorize;

        // pre- operation
        readonly IBefore before;

        readonly IBeforeAsync beforeAsync;

        // post- operation
        readonly IAfter after;

        readonly IAfterAsync afterAsync;

        internal Nodule(string name, ICustomAttributeProvider attrp, UiAttribute ui = null, AuthorizeAttribute authorize = null)
        {
            this.name = name ?? throw new ServiceException("null nodule name");
            this.capital = !string.IsNullOrEmpty(name) && char.IsUpper(name[0]);
            this.lower = name.ToLower();
            // either methodinfo or typeinfo
            if (attrp == null)
            {
                attrp = GetType().GetTypeInfo();
            }
            // retrieve Ui annotation 
            if (ui == null)
            {
                var uis = (UiAttribute[]) attrp.GetCustomAttributes(typeof(UiAttribute), true);
                ui = uis.Length > 0 ? uis[0] : null;
            }
            this.label = ui?.Label ?? name.ToUpper();
            this.tip = ui?.Tip ?? label;
            this.flag = ui?.Flag ?? 0;
            // authorize
            if (authorize == null)
            {
                var accs = (AuthorizeAttribute[]) attrp.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                if (accs.Length > 0)
                {
                    authorize = accs[0];
                }
            }
            this.authorize = authorize;
            // filters
            var attrs = attrp.GetCustomAttributes(true);
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

        public bool IsCapital => capital;

        public string Lower => lower;

        public string Label => label;

        public string Tip => tip;

        public byte Flag => flag;

        public AuthorizeAttribute Authorize => authorize;

        public IBefore Before => before;

        public IBeforeAsync BeforeAsync => beforeAsync;

        public IAfter After => after;

        public IAfterAsync AfterAsync => afterAsync;

        public bool HasAuthorize => authorize != null;

        public bool DoAuthorize(WebContext wc, bool strict)
        {
            if (authorize != null)
            {
                if (wc.Principal == null && strict)
                {
                    return false;
                }
                return authorize.Check(wc);
            }
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }
}