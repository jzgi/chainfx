using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/mgt/
	///
    public class WwwFameZoneMgtSub : WebSub<Fame>
    {
        public WwwFameZoneMgtSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc, Fame zone)
        {
            throw new System.NotImplementedException();
        }
    }
}