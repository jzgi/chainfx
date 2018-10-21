using System.Reflection;

namespace Greatbone
{
    /// <summary>
    /// A certain node of resources along URI path.
    /// </summary>
    public abstract class Nodule : IKeyable<string>
    {
        // uri segment
        readonly string name;

        // name in lowercase
        readonly string lower;

        readonly string label;

        readonly string icon;

        readonly string tip;

        readonly byte group;

        // permission check
        internal readonly AuthorizeAttribute authorize;

        internal readonly AuthorizeException except;

        internal Nodule(string name, ICustomAttributeProvider attrp, UiAttribute ui = null, AuthorizeAttribute authorize = null)
        {
            this.name = name ?? throw new ServiceException("null nodule name");
            this.lower = name.ToLower();

            // either methodinfo or typeinfo
            if (attrp == null)
            {
                attrp = GetType().GetTypeInfo();
            }

            if (ui == null)
            {
                var uis = (UiAttribute[]) attrp.GetCustomAttributes(typeof(UiAttribute), true);
                if (uis.Length > 0) ui = uis[0];
            }
            label = ui?.Label;
            icon = ui?.Icon;
            tip = ui?.Tip ?? label;
            group = ui?.Group ?? 0;

            if (authorize == null)
            {
                var aas = (AuthorizeAttribute[]) attrp.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                if (aas.Length > 0)
                {
                    authorize = aas[0];
                }
            }
            if (authorize != null)
            {
                this.authorize = authorize;
            }
            except = new AuthorizeException(this, authorize);
        }

        public virtual string Key => name;

        public string Lower => lower;

        public string Label => label;

        public string Icon => icon;

        public string Tip => tip;

        public byte Group => group;

        public bool DoAuthorize(WebContext wc)
        {
            return authorize == null || authorize.Do(wc);
        }

        public override string ToString()
        {
            return name;
        }
    }
}