using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The business operation service.
    ///
    public class OpService : AbstService
    {
        static readonly Client WeiXin = new Client("http://sh.api.weixin.qq.com");

        static readonly Client WCPay = new Client("https://api.mch.weixin.qq.com");


        public OpService(FolderContext fc) : base(fc)
        {
            Create<ShopFolder>("shop");

            Create<UserFolder>("user");

            Create<RepayFolder>("repay");
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

        ///
        /// redirect_uri/?code=CODE&amp;state=STATE
        public async Task wxsignon(ActionContext ac)
        {
            string code = ac.Query[nameof(code)];
            if (code == null)
            {
                ac.Reply(301);
                return;
            }
            string openid = ac.Cookies[nameof(openid)];
            string nickname = ac.Cookies[nameof(nickname)];
            if (openid == null || nickname == null)
            {
                // get access token by the code
                JObj jo = await WeiXin.GetAsync<JObj>(null, "/sns/oauth2/access_token?appid=APPID&secret=SECRET&code=CODE&grant_type=authorization_code");

                string access_token = jo[nameof(access_token)];
                openid = jo[nameof(openid)];

                // get user info
                jo = await WeiXin.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                nickname = jo[nameof(nickname)];

                using (var dc = ac.NewDbContext())
                {
                    dc.Execute();
                }

                ac.SetHeader("Set-Cookie", "openid=" + openid);
            }

            // display index.html
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

        [Ui("登录")]
        public async Task signon(ActionContext ac)
        {
            if (ac.GET) // return the login form
            {
                var login = new Login();
                ac.ReplyForm(200, login);
            }
            else // POST
            {
                var login = await ac.ReadObjectAsync<Login>();
                string credential = login.CalcCredential();

                if (login.IsShop)
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM shops WHERE id = @1", (p) => p.Set(login.id)))
                        {
                            var shop = dc.ToObject<Shop>();
                            if (credential.Equals(shop.credential))
                            {
                                SetBearerCookie(ac, shop.ToToken());
                                ac.ReplyRedirect(login.orig);
                                return;
                            }
                            else { ac.Reply(400); }
                        }
                        else { ac.Reply(404); }
                    }
                }
                else if (login.IsUser)
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(login.id)))
                        {
                            var user = dc.ToObject<User>();
                            if (credential.Equals(user.credential))
                            {
                                SetBearerCookie(ac, user.ToToken());
                                ac.ReplyRedirect(login.orig);
                                return;
                            }
                            else { ac.Reply(400); }
                        }
                        else { ac.Reply(404); }
                    }
                }
                else // is admin id
                {
                    var admin = admins.Find(a => a.id == login.id && credential.Equals(a.credential));
                    if (admin != null)
                    {
                        SetBearerCookie(ac, admin.ToToken());
                        ac.ReplyRedirect(login.orig);
                        return;
                    }
                    else { ac.Reply(404); }
                }

                // error
                ac.ReplyForm(200, login);
            }
        }

        public void ACCESS_TOKEN(EventContext ec)
        {

        }
    }
}