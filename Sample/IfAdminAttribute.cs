using Greatbone.Core;

namespace Greatbone.Sample
{
    public class IfAdminAttribute : IfAttribute
    {
        public override bool Check(WebContext wc, string subscpt)
        {
            Token tok = (Token)wc.Token;
            return tok.admin;
        }
    }
}