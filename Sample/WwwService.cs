using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The website service controller.
    /// </summary>
    ///
    public class WwwService : AbsService
    {
        public WwwService(WebConfig cfg) : base(cfg)
        {
            AddSub<MySub>("my", true);
        }


        ///
        /// <summary>
        /// Get all fame categories.
        /// </summary>
        /// <code>
        /// GET /cats
        /// </code>
        ///
        public void cats(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                if (dc.Query("SELECT * FROM cats WHERE NOT disabled"))
                {
                    Cat[] arr = dc.ToArr<Cat>();
                    wc.Respond(200, arr);
                }
                else
                {
                    wc.StatusCode = 204;
                }

            }
        }

        public void search(WebContext wc)
        {
        }

        public void contact(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                dc.Begin();

                //				sc.DoNonQuery("inaert", o => o.ToString(););

                // msg

                dc.Commit();
            }

            wc.StatusCode = 200;
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