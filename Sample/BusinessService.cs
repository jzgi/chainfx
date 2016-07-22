using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BusinessService : WebService
    {
        public BusinessService(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}