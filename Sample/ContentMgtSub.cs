using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ContentMgtSub : WebSub
    {
        public ContentMgtSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}