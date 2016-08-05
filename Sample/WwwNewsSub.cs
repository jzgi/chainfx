using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /news/
	///
    public class WwwNewsSub : WebSub
    {
        public WwwNewsSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}