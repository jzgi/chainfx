using Greatbone.Core;

namespace Greatbone.Sample
{
    public class DirectoryAdminSub : WebSub
    {
        public DirectoryAdminSub(WebHub hub) : base(hub)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}