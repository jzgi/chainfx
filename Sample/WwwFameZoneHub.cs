using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// /fame/-/
	///
	public class WwwFameZoneHub : WebZoneHub<Fame>
	{
		public WwwFameZoneHub(WebServiceBuilder builder) : base(builder)
		{
			AddSub<WwwFameZoneMgtSub>("mgt", null);
		}


		public override void Default(WebContext wc, Fame zone)
		{
			throw new System.NotImplementedException();
		}
	}
}