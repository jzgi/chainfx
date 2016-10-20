using Greatbone.Core;

namespace Greatbone.Sample
{
    public class IfAdminAttribute : IfAttribute
    {
        public override bool Check(WebContext wc, string var)
        {
            Token tok = (Token)wc.Token;
            return tok.admin;
        }
    }
}