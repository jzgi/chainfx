using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	public class WwwFameService : WebService
	{
		public WwwFameService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<WwwFameZoneHub, Fame>(null);

		}
	}
}