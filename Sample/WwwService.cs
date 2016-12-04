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
        public void accestoken(WebActionContext ac)
        {

        }

        public void search(WebActionContext e)
        {

        }

        void All(params WebClientContext[] calls)
        {

        }

        //
        // EVENTS
        // 

        public void DAILY_RPT(WebEventContext ec)
        {
            using (var dc = ec.NewDbContext())
            {
                dc.Begin();

                // insert

                // update lastid

                dc.Commit();
            }
        }
    }
}