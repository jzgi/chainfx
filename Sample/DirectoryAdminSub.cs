using Greatbone.Core;

namespace Greatbone.Sample
{
    public class DirectoryAdminSub : WebSub
    {
        public DirectoryAdminSub(WebService service) : base(service)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}