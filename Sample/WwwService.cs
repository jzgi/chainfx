using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The website service controller.
	///
	public class WwwService : WebService
	{
		public WwwService(WebServiceBuilder builder) : base(builder)
		{
			AddSub<WwwMySub>("my", null);

			AddSub<WwwNewsSub>("news", null);

			AddSub<WwwEventSub>("event", null);

			AddSub<WwwFameService>("fame", null);

			AddSub<WwwBrandService>("brand", null);

		}

		public void Show(WebContext wc)
		{
		}

		public void Contact(WebContext wc)
		{
			using (var sc = NewDbContext())
			{
				sc.BeginTransaction();

//				sc.DoNonQuery("inaert", o => o.ToString(););

				// msg

				sc.CommitTransaction();
			}

			wc.Response.StatusCode = 200;
		}
	}
}