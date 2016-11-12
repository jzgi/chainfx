using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service controller.
    ///
    public class WwwService : AbstService
    {
        public WwwService(WebConfig cfg) : base(cfg)
        {
            Add<MyDir>("my");
        }


        ///
        /// Get all fame categories.
        ///
        /// <code>
        /// GET /cats
        /// </code>
        ///
        public void cats(WebContext wc, string subscpt)
        {
            using (var dc = NewDbContext())
            {
                if (dc.Query("SELECT * FROM cats WHERE NOT disabled"))
                {
                    Cat[] arr = dc.ToDatas<Cat>();
                    wc.SendJson(200, arr);
                }
                else
                {
                    wc.StatusCode = 204;
                }
            }
        }

        public void search(WebContext wc, string subscpt)
        {
        }

        //
        // MESSAGES
        // 

        public void USER_UPD(MsgContext mc)
        {
        }

        public void RPT_OK(MsgContext mc)
        {
        }
    }
}