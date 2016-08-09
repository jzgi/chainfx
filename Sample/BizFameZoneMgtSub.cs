using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/mgt/
	///
    public class BizFameZoneMgtSub : WebSub<Fame>
    {
        public BizFameZoneMgtSub(WebServiceBuilder builder) : base(builder)
        {
        }

        public override void Default(WebContext wc, Fame zone)
        {
            throw new System.NotImplementedException();
        }
    }
}