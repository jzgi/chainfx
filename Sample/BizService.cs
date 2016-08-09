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

		public BizService(WebServiceBuilder builder) : base(builder)
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