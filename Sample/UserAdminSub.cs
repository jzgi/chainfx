using Greatbone.Core;

namespace Greatbone.Sample
{
    public class DirectoryAdminSub : WebSub
    {
        public DirectoryAdminSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}