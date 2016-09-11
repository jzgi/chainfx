using Greatbone.Core;

namespace Greatbone.Sample
{
	///
	///
	[Publish("topicname")]
	public class FameService : WebService
	{
		public FameService(WebServiceConfig cfg) : base(cfg)
		{
			SetXHub<FameXHub>(false);
		}

		        /// <summary>
        /// Gets the top fames. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void Default(WebContext wc)
        {
            int page = 0;
            wc.Request.GetParam("page", ref page);

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY  LIMIT 20 OFFSET @offset", p => p.Set("@offset", page * 20)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

        public void Top(WebContext wc)
        {
            int page = 0;
            wc.Request.GetParam("page", ref page);

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY rating LIMIT 20 OFFSET @offset", p => p.Set("@offset", page * 20)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }


	}
}