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

        readonly string tip;

        readonly byte group;

        // access check
        internal readonly AuthAttribute auth;

        // pre-action and post-action operation
        internal readonly FilterAttribute filter;

        internal Nodule(string name, ICustomAttributeProvider attrp, UiAttribute ui = null, AuthAttribute auth = null)
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
            this.label = ui?.Label ?? name.ToUpper();
            this.tip = ui?.Tip ?? label;
            this.group = ui?.Group ?? 0;

            if (auth == null)
            {
                var aas = (AuthAttribute[]) attrp.GetCustomAttributes(typeof(AuthAttribute), true);
                if (aas.Length > 0) this.auth = aas[0];
            }

            var fas = (FilterAttribute[]) attrp.GetCustomAttributes(typeof(FilterAttribute), true);
            if (fas.Length > 0) filter = fas[0];
        }

        public virtual string Key => name;

        public string Lower => lower;

        public string Label => label;

        public string Tip => tip;

        public byte Group => group;

        public AuthAttribute Auth => auth;

        public FilterAttribute Filter => filter;

        public bool HasAuthorize => auth != null;

        public bool CheckAccess(WebContext wc, out AccessException except)
        {
            if (auth != null)
            {
                bool result = auth.Authenticate(wc);
                if (result)
                {
//                    except = new AccessException(result, auth);
//                    return false;
                }
            }
            except = null;
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }
}