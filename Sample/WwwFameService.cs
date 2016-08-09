using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	public class WwwFameService : WebService
	{
		public WwwFameService(WebServiceBuilder builder) : base(builder)
		{
			MountHub<WwwFameZoneHub, Fame>(null);

		}
	}
}