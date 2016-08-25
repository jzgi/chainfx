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
			AddSub<WwwMySub>("my", false);

			//

			Subscribe("amcom", x =>
				{
					string s = x.GetType().Name;
				}
			);
		}

		///
		/// <summary>Returns the default HTML page</summary>
		public override void Default(WebContext wc)
		{
			base.Default(wc);
		}

		public void Home(WebContext wc)
		{
		}

		public void Posts(WebContext wc)
		{
		}

		public void Notices(WebContext wc)
		{
		}

		public void Search(WebContext wc)
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