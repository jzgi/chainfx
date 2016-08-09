using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAdminSub : WebSub
    {
        public UserAdminSub(WebServiceBuilder builder) : base(builder)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}