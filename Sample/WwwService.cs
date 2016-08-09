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
		}

		public void Show(WebContext wc)
		{
		}

		public void Contact(WebContext wc)
		{
			using (var sc = NewSqlContext())
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