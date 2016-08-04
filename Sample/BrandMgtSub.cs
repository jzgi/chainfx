using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BrandMgtSub : WebSub
    {
        public BrandMgtSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}