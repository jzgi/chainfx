using Greatbone.Core;

namespace Greatbone.Sample
{
    public class BusinessMgtSub : WebSub
    {
        public BusinessMgtSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}