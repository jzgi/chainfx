using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The website service controller.
    /// </summary>
    ///
    public class WwwServiceWork : AbstServiceWork
    {
        public WwwServiceWork(WebConfig cfg) : base(cfg)
        {
            AddChild<MyWork>("my");
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
                    Cat[] arr = dc.ToBeans<Cat>();
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