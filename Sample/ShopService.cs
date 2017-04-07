using System.Collections.Generic;
using System.Threading;
using Greatbone.Core;
using System.Threading.Tasks;

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

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task notify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            string appid = xe[nameof(appid)];
            string mch_id = xe[nameof(mch_id)];
            string openid = xe[nameof(openid)];
            string nonce_str = xe[nameof(nonce_str)];
            string sign = xe[nameof(sign)];
            string result_code = xe[nameof(result_code)];

            string bank_type = xe[nameof(bank_type)];
            string total_fee = xe[nameof(total_fee)]; // 订单总金额单位分
            string cash_fee = xe[nameof(cash_fee)]; // 支付金额单位分
            string transaction_id = xe[nameof(transaction_id)]; // 微信支付订单号
            string out_trade_no = xe[nameof(out_trade_no)]; // 商户订单号
            string time_end = xe[nameof(time_end)]; // 支付完成时间

        }

    }
}