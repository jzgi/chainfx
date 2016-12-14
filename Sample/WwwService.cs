using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The website service.
    ///
    public class WwwService : WebService
    {
        static readonly WebClient WeChat = new WebClient("wechat", "http://sh.api.weixin.qq.com");

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

        public async void search(WebActionContext ac)
        {
            Arr arr = await WeChat.GetArrAync(ac, "/");

            // shops.CallAll()

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