using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/mgt/
	///
    public class ContentFameZoneMgtSub : WebSub<Fame>
    {
        public ContentFameZoneMgtSub(WebServiceContext wsc) : base(wsc)
        {
        }

        public override void Default(WebContext wc, Fame zone)
        {
            throw new System.NotImplementedException();
        }
    }
}