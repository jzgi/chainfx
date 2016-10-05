using Greatbone.Core;

namespace Greatbone.Sample
{
    public class FameHub : WebHub
    {
        public FameHub(WebBuild cfg) : base(cfg)
        {
            SetVarHub<FameVarHub>(false);
        }

        /// <summary>
        /// Gets the top list of fames. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void @default(WebContext wc)
        {
            int page = 0;
            wc.Get("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY  LIMIT 20 OFFSET @offset",
                    p => p.Put("@offset", page * 20)))
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

        public void top(WebContext wc)
        {
            int page = 0;
            wc.Get("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM fames WHERE ORDER BY rating LIMIT 20 OFFSET @offset",
                    p => p.Put("@offset", page * 20)))
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