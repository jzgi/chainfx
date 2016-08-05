using Greatbone.Core;

namespace Greatbone.Sample
{
    public class UserAdminSub : WebSub
    {
        public UserAdminSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}