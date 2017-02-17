using System.Threading;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class CommService : AbstService
    {
        readonly WebClient WeiXin = new WebClient("weixin", "https://api.weixin.qq.com");

        // the timer for triggering periodically obtaining access_token from weixin
        readonly Timer timer;

        volatile string access_token;

        public CommService(WebServiceContext sc) : base(sc)
        {
            // add sub folder
            CreateVar<CommVarFolder>();

            // timer obtaining access_token from weixin
            timer = new Timer(async state =>
            {
                JObj jo = await WeiXin.GetAsync<JObj>(null, "cgi-bin/token?grant_type=client_credential&appid=APPID&secret=APPSECRET");

                if (jo == null) return;

                access_token = jo[nameof(access_token)];
                if (access_token == null)
                {
                    ERR("error getting access token");
                    string errmsg = jo[nameof(errmsg)];
                    ERR(errmsg);
                    return;
                }

                int expires_in = jo[nameof(expires_in)]; // in seconds

                int millis = (expires_in - 60) * 1000;
                timer.Change(millis, millis); // adjust interval

                // queue an event
                using (var dc = NewDbContext())
                {
                    // dc.Event("ACCESS_TOKEN", null, access_token.ToString());
                }

            }, null, 5000, 60000);
        }


    }
}