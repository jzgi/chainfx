using Greatbone.Core;

namespace Greatbone.Sample
{
    public class DirectoryAdminSub : WebSub
    {
        public DirectoryAdminSub(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}