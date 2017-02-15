using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ShopAttribute : RoleAttribute
    {
        readonly bool owner;

        public ShopAttribute(bool owner = true)
        {
            this.owner = owner;
        }

        public override bool Check(WebActionContext ac)
        {
            string shopid = ac[typeof(ShopVarFolder)];

            Token tok = (Token)ac.Token;

            return tok.roles == 2 && shopid.Equals(tok.extra);
        }
    }
}