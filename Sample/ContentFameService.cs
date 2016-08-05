using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/
	///
	public class ContentFameService : WebService
	{
		public ContentFameService(WebServiceContext wsc) : base(wsc)
		{
			MountHub<ContentFameZoneHub, Fame>(null);

		}
	}
}