using System.Collections.Generic;
using System.Threading;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ShopService : AbstService
    {
        // the timer for triggering periodically obtaining access_token from weixin
        readonly Timer timer;

        volatile string access_token;

        public ShopService(ServiceContext sc) : base(sc)
        {
            Create<PubShopWork>("pub");

            Create<MyUserWork>("my");

            Create<MgrShopWork>("mgr");

            Create<AdmWork>("adm");

            // timer obtaining access_token from weixin
            // timer = new Timer(async state =>
            // {
            //     JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/cgi-bin/token?grant_type=client_credential&appid=" + weixin.appid + "&secret=" + weixin.appsecret);

            //     if (jo == null) return;

            //     access_token = jo[nameof(access_token)];
            //     if (access_token == null)
            //     {
            //         ERR("error getting access token");
            //         string errmsg = jo[nameof(errmsg)];
            //         ERR(errmsg);
            //         return;
            //     }

            //     int expires_in = jo[nameof(expires_in)]; // in seconds

            //     int millis = (expires_in - 60) * 1000;
            //     timer.Change(millis, millis); // adjust interval

            //     // post an event
            //     // using (var dc = NewDbContext())
            //     // {
            //     //     dc.Post("ACCESS_TOKEN", null, access_token, null);
            //     // }

            // }, null, 5000, 60000);

        }

        public void @default(ActionContext ac)
        {
            DoFile("default.html", ac);
        }

    }
}