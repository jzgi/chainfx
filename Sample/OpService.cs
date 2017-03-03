using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The business operation service.
    ///
    public class OpService : AbstService
    {
        static readonly Client WCPay = new Client("https://api.mch.weixin.qq.com");

        // the timer for triggering periodically obtaining access_token from weixin
        readonly Timer timer;

        volatile string access_token;


        public OpService(ServiceContext sc) : base(sc)
        {
            Create<ShopFolder>("shop");

            Create<UserFolder>("user");

            Create<RepayFolder>("repay");

            // timer obtaining access_token from weixin
            timer = new Timer(async state =>
            {
                JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/cgi-bin/token?grant_type=client_credential&appid=" + weixin.appid + "&secret=" + weixin.appsecret);

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

                // post an event
                // using (var dc = NewDbContext())
                // {
                //     dc.Post("ACCESS_TOKEN", null, access_token, null);
                // }

            }, null, 5000, 60000);

        }

        [Check]
        public void @default(ActionContext ac)
        {
            Token tok = (Token)ac.Token;
            if (tok.IsAdmin)
            {
                // display the folder's index page for admin
                ac.ReplyFolderPage(200, (List<Shop>)null);
            }
            else if (tok.IsShop)
            {
                // redirect to the shop's home page
                string shopid = tok.extra;
                ac.ReplyRedirect("shop/" + shopid + "/");
            }
            else if (tok.IsUser)
            {
                // redirect to user's home page
                string userwx = tok.wx;
                ac.ReplyRedirect("user/" + userwx + "/");
            }
        }


        public async Task paynotify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            string mch_id = xe[nameof(mch_id)];
            string openid = xe[nameof(openid)];
            string bank_type = xe[nameof(bank_type)];
            string total_fee = xe[nameof(total_fee)];
            string transaction_id = xe[nameof(transaction_id)]; // 微信支付订单号
            string out_trade_no = xe[nameof(out_trade_no)]; // 商户订单号

        }

        public void ACCESS_TOKEN(EventContext ec)
        {

        }
    }
}