using System.Reflection;

namespace Greatbone.Service
{
    /// <summary>
    /// A web handling target.
    /// </summary>
    public abstract class WebTarget : IKeyable<string>
    {
        // uri segment
        readonly string name;

        // name in lowercase
        readonly string lower;

        readonly string label;

        readonly string tip;

        readonly byte sort;

        // permission check
        internal readonly AuthorizeAttribute authorize;

        internal readonly AuthorizeException exception;

        internal WebTarget(string name, ICustomAttributeProvider attrp, UiAttribute ui = null, AuthorizeAttribute authorize = null)
        {
            this.name = name ?? throw new WebException("null target name");
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
            tip = ui?.Tip ?? label;
            sort = ui?.Sort ?? 0;

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
            exception = new AuthorizeException(this, authorize);
        }

        public virtual string Key => name;

        public string Lower => lower;

        public string Label => label;

        public string Tip => tip;

        public byte Sort => sort;

        public bool DoAuthorize(WebContext wc)
        {
            if (authorize != null)
            {
                // check if trusted peer
                if (wc.CallerSign != null && wc.CallerSign == Host.Sign)
                {
                    return true; // trusted without further check
                }
                return authorize.Do(wc);
            }
            return true;
        }

        public override string ToString()
        {
            return name;
        }
    }
}