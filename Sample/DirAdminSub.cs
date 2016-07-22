using Greatbone.Core;

namespace Greatbone.Sample
{
    public class DirAdminSub : WebSub
    {
        public DirAdminSub(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}