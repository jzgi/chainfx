using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAttribute : AuthorizeAttribute
    {
        readonly string oprat;

        readonly string dvrat;

        readonly string mgrat;

        public UserAttribute() : this(null, null)
        {
        }

        public UserAttribute(string oprat, string mgrat)
        {
            this.oprat = oprat;
            this.mgrat = mgrat;
        }

        public bool IsOpr => oprat != null;

        public bool IsAdm => mgrat != null;

        public override bool Check(ActionContext ac)
        {
            User prin = ac.Principal as User;

            if (prin == null) return false;

            if (oprat != null)
            {
                if (prin.oprat == null)
                {
                    return false;
                }
                string shopid = ac[typeof(ShopVarWork)];
                return shopid == prin.oprat;
            }
            if (mgrat != null)
            {
                if (prin.mgrat == null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}