using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	public class BizFameService : WebService
	{
		public BizFameService(WebServiceBuilder builder) : base(builder)
		{
			MountHub<BizFameZoneHub, Fame>(null);

		}
	}
}