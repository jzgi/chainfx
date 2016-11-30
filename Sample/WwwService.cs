using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service.
    ///
    public class WwwService : WebService
    {
        const string api = "sh.api.weixin.qq.com";

        public WwwService(WebConfig cfg) : base(cfg)
        {
        }


        ///
        /// Get access token for WeChat.
        ///
        /// <code>
        /// GET /cats
        /// </code>
        ///
        public void accestoken(WebContext wc, string subscpt)
        {

        }

        public void search(WebContext wc, string subscpt)
        {

        }

        void All(params WebCall[] calls)
        {

        }

        //
        // EVENTS
        // 

        public void DAILY_RPT(WebEvent we)
        {
            using (var dc = NewDbContext())
            {
                dc.Begin();

                // insert

                // update lastid

                dc.Commit();
            }
        }
    }
}