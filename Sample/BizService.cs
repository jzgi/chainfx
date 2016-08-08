using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The content management service.
	///
	public class BizService : WebService
	{
		//
		// INIT
		//

		public BizService(WebServiceContext wsc) : base(wsc)
		{
			AddSub<BizNewsSub>("news", null);

			AddSub<BizEventSub>("event", null);

			AddSub<BizFameService>("fame", null);

			AddSub<BizBrandService>("brand", null);
		}

		//
		// REQUEST HANDLING
		//

		//
		// EVENT HANDLING
		//

		public void OnEnroll()
		{
		}
	}
}