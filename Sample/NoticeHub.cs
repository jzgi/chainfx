using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// <summary>The notice service.</summary>
    ///
    public class NoticeHub : WebHub
    {
        public NoticeHub(WebConfig cfg) : base(cfg)
        {
            SetVarHub<NoticeVarHub>(false);
        }

        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        /// <param name="page">page number</param>
        public override void @default(WebContext wc)
        {
            int page = 0;
            wc.Get("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("SELECT * FROM notices WHERE duedate <= current_date ORDER BY id LIMIT 20 OFFSET @offset", p => p.Put("@offset", page * 20)))
                {

                }
                else
                {
                    wc.Response.StatusCode = 204;
                }
            }
        }

        /// <summary>
        /// Gets the specified top page from the notices table. 
        /// </summary>
        public void New(WebContext wc)
        {
            Obj o = (Obj)wc.Data;
            int age = o[nameof(age)];

            int page = 0;
            wc.Get("page", ref page);

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query("INSERT INTO notices () VALUES ()",
                    p => { p.Put(page * 20); p.Put(page * 20); }))
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