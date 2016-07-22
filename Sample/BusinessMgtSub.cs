using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BusinessMgtSub : WebSub
    {
        public BusinessMgtSub(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}