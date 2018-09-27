using System.Reflection;
using System.Threading.Tasks;

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

        // authentication & authorization
        internal readonly AccessAttribute access;

        internal readonly AccessException except;

        // pre-action and post-action operation
        internal readonly FilterAttribute filter;

        internal Nodule(string name, ICustomAttributeProvider attrp, UiAttribute ui = null, AccessAttribute auth = null)
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
                var aas = (AccessAttribute[]) attrp.GetCustomAttributes(typeof(AccessAttribute), true);
                if (aas.Length > 0)
                {
                    auth = aas[0];
                }
            }
            if (auth != null)
            {
                this.access = auth;
                except = new AccessException(auth);
            }

            var fas = (FilterAttribute[]) attrp.GetCustomAttributes(typeof(FilterAttribute), true);
            if (fas.Length > 0) filter = fas[0];
        }

        public virtual string Key => name;

        public string Lower => lower;

        public string Label => label;

        public string Tip => tip;

        public byte Group => group;

        public AccessAttribute Access => access;

        public FilterAttribute Filter => filter;

        public bool AccessRequired => access != null;

        public bool AccessAsync => access.IsAsync;

        public bool AccessSync => !access.IsAsync;

        public bool CheckAccess(WebContext wc)
        {
            if (access != null)
            {
                if (!wc.Authenticated) // if not yet authenticated
                {
                    if (access.Authenticate(wc))
                    {
                        return false;
                    }
                    wc.Authenticated = true;
                }
                if (!access.Authorize(wc))
                {
                    throw except;
                }
            }
            return true;
        }

        public async Task<bool> CheckAccessAsync(WebContext wc)
        {
            if (access != null)
            {
                if (!wc.Authenticated) // if not yet authenticated
                {
                    if (!await access.AuthenticateAsync(wc))
                    {
                        return false;
                    }
                    wc.Authenticated = true;
                }
                if (!access.Authorize(wc))
                {
                    throw except;
                }
            }
            return true;
        }

        public bool Authorize(WebContext wc)
        {
            return access == null || access.Authorize(wc);
        }

        public override string ToString()
        {
            return name;
        }
    }
}