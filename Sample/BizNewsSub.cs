using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /news/
	///
    public class BizNewsSub : WebSub
    {
        public BizNewsSub(WebServiceBuilder builder) : base(builder)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}