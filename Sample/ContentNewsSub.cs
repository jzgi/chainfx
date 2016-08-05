using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /news/
	///
    public class ContentNewsSub : WebSub
    {
        public ContentNewsSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}