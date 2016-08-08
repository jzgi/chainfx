using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	public class BizFameService : WebService
	{
		public BizFameService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<BizFameZoneHub, Fame>(null);

		}
	}
}