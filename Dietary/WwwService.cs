using Greatbone.Core;

namespace Ministry.Dietary
{

    ///
    /// <summary>
    /// The website service controller.
    /// </summary>
    ///
    public class WwwService : WebService
    {

        public WwwService(WebConfig cfg) : base(cfg)
        {
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