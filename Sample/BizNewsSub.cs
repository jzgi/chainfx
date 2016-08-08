using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /news/
	///
    public class BizNewsSub : WebSub
    {
        public BizNewsSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}