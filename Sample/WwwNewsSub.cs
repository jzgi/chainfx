using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /news/
	///
    public class WwwNewsSub : WebSub
    {
        public WwwNewsSub(WebServiceBuilder builder) : base(builder)
        {
        }

        public override void Default(WebContext wc)
        {
            throw new System.NotImplementedException();
        }
    }
}