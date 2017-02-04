using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ToShopAttribute : ToAttribute
    {
        public override bool Check(WebActionContext ac)
        {
            string shopid = ac[typeof(ShopVarFolder)];

            Token tok = (Token)ac.Token;

            return tok.role == 2 && shopid.Equals(tok.extra);
        }
    }
}