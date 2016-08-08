using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	/// The website service controller.
	///
	public class WwwService : WebService
	{
		public WwwService(WebServiceContext wsc) : base(wsc)
		{
			AddSub<WwwMySub>("my", null);
		}

		public void Show(WebContext wc)
		{
			Fame v = new Fame()
			{
				key = "123",
				name = "luke"
			};
			v.To(wc.Response);
		}

		public void Contact(WebContext wc)
		{
			using (var sc = NewSqlContext())
			{
				sc.BeginTransaction();

				sc.DoNonQuery("inaert", o => o.Put("abc", 23));

				// msg

				sc.CommitTransaction();
			}

			wc.Response.StatusCode = 200;
		}
	}
}