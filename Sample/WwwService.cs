using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service controller.
    ///
    public class WwwService : WebService
    {
        public WwwService(WebConfig cfg) : base(cfg)
        {
            AddSub<MySub>("my", true);

        }


        ///
        /// GET /cats
        public void cats(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                if (dc.Query("SELECT * FROM cats WHERE NOT disabled", null))
                {
                    Cat[] arr = dc.GetArr<Cat>();
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
    }
}