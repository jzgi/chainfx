using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FameZone : WebZone
    {
        public FameZone(WebConfig cfg) : base(cfg)
        {
            SetVarHub<FameVarHub>(false);
        }

        /// <summary>
        /// Gets the top list of fames. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void Default(WebContext wc)
        {
            int page;
            wc.GetParam("page", out page);

            using (var dc = Service.NewSqlContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY  LIMIT 20 OFFSET @offset", p => p.Set("@offset", page * 20)))
                {
                    while (dc.NextRow())
                    {
                    }
                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

        public void Top(WebContext wc)
        {
            int page;
            wc.GetParam("page", out page);

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