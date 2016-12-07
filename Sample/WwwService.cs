using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service.
    ///
    public class WwwService : WebService
    {
        const string api = "sh.api.weixin.qq.com";

        readonly WebClient[] shops;

        public WwwService(WebConfig cfg) : base(cfg)
        {
            shops = Clients.All("shop-");
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

        public void search(WebActionContext ac)
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