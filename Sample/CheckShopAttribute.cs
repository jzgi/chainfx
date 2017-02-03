using Greatbone.Core;

namespace Greatbone.Sample
{
    public class CheckShopAttribute : CheckAttribute
    {
        public override bool Check(WebActionContext ac)
        {
            string shopid = ac[typeof(ShopVarFolder)];

            Token tok = (Token)ac.Token;

            return tok.role == 2 && shopid.Equals(tok.extra);
        }
    }
}