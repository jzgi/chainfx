using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The content management service.
	///
	public class ContentService : WebService
	{
		//
		// INIT
		//

		public ContentService(WebServiceContext wsc) : base(wsc)
		{
			AddSub<ContentNewsSub>("news", null);

			AddSub<ContentEventSub>("event", null);

			AddSub<ContentFameService>("fame", null);

			AddSub<ContentBrandService>("brand", null);
		}

		//
		// REQUEST HANDLING
		//

		public override void Default(WebContext wc)
		{
			throw new System.NotImplementedException();
		}

		//
		// EVENT HANDLING
		//

		public void OnEnroll()
		{
		}
	}
}