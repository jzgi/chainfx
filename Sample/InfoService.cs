using Greatbone.Core;

namespace Greatbone.Sample
{
    public class InfoService : WebService
    {
        public InfoService(WebCreationContext wcc) : base(wcc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}