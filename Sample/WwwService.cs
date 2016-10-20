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
            AddControl<MySub>("my", true);
        }


        ///
        /// <summary>
        /// Get all fame categories.
        /// </summary>
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
                    Cat[] arr = dc.ToArr<Cat>();
                    wc.Out(200, arr);
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