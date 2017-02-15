using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using NpgsqlTypes;
using System;

namespace Greatbone.Sample
{
    ///
    /// The business operation service.
    ///
    public class OpService : AbstService
    {
        static readonly WebClient WeiXin = new WebClient("wechat", "http://sh.api.weixin.qq.com");

        static readonly WebClient WCPay = new WebClient("wcpay", "https://api.mch.weixin.qq.com");


        public OpService(WebServiceContext sc) : base(sc)
        {
            Create<ShopFolder>("shop");

            Create<UserFolder>("user");

            Create<RepayFolder>("repay");
        }

        [Role]
        public void @default(WebActionContext ac)
        {
            Token tok = (Token)ac.Token;
            if (tok.IsAdmin)
            {
                // display the folder's index page for admin
                ac.ReplyGridPage(200, (List<Shop>)null);
            }
            else if (tok.IsShop)
            {
                // redirect to the shop's home page
                string shopid = tok.extra;
                ac.SetHeader("Location", "shop/" + shopid + "/");
                ac.Reply(303);
            }
            else if (tok.IsUser)
            {
                // redirect to user's home page
                string userwx = tok.wx;
                ac.SetHeader("Location", "user/" + userwx + "/");
                ac.Reply(303);
            }
        }

        ///
        /// redirect_uri/?code=CODE&amp;state=STATE
        public async Task weixin(WebActionContext ac)
        {
            string code = ac.Query[nameof(code)];
            if (code == null)
            {
                // redirect the user to weixin authorization page
                ac.SetHeader("Location", "https://open.weixin.qq.com/connect/oauth2/authorize?appid=APPID&redirect_uri=REDIRECT_URI&response_type=code&scope=SCOPE&state=STATE#wechat_redirect");
                ac.Reply(302);
            }
            else
            {
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

                    ac.SetHeader("Set-Cookie", "openid=" + openid);
                }

                // display index.html
            }
        }


        public async Task paynotify(WebActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            string mch_id = xe[nameof(mch_id)];
            string openid = xe[nameof(openid)];
            string bank_type = xe[nameof(bank_type)];
            string total_fee = xe[nameof(total_fee)];
            string transaction_id = xe[nameof(transaction_id)]; // 微信支付订单号
            string out_trade_no = xe[nameof(out_trade_no)]; // 商户订单号

        }


        public async Task signon(WebActionContext ac)
        {
            if (ac.GET) // return the login form
            {
                Form q = ac.Query;
                string id = q[nameof(id)];
                string password = q[nameof(password)];
                string orig = q[nameof(orig)];
                bool remember = q[nameof(remember)];
                ac.ReplyForm(200, h =>
                {
                    h.TEXT(nameof(id), id, Required: true, Max: 11, Placeholder: "用户手机号，或者商户编号");
                    h.PASSWORD(nameof(password), password);
                    h.HIDDEN(nameof(orig), orig);
                    h.CHECKBOX(nameof(remember), remember);
                });

            }
            else // POST
            {
                Form f = await ac.ReadAsync<Form>();
                string id = f[nameof(id)];
                string password = f[nameof(password)];
                string orig = f[nameof(orig)];
                bool remember = f[nameof(remember)];
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                {
                    ac.Reply(400); // bad request
                    return;
                }

                if (id.Length == 6 && char.IsDigit(id[0])) // is shopid
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM shops WHERE id = @1", (p) => p.Set(id)))
                        {
                            var shop = dc.ToObject<Shop>();
                            string credential = TextUtility.MD5(id + ':' + password);
                            if (credential.Equals(shop.credential))
                            {
                                Service.Authent.SetCookieHeader(ac, shop.ToToken());
                                goto Redirect;
                            }
                            else { ac.Reply(400); }
                        }
                        else { ac.Reply(404); }
                    }
                }
                else if (id.Length == 11 && char.IsDigit(id[0])) // is userid
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(id)))
                        {
                            var user = dc.ToObject<User>();
                            string credential = TextUtility.MD5(id + ':' + password);
                            if (credential.Equals(user.credential))
                            {
                                Service.Authent.SetCookieHeader(ac, user.ToToken());
                                goto Redirect;
                            }
                            else { ac.Reply(400); }
                        }
                        else { ac.Reply(404); }
                    }
                }
                else // is admin id
                {
                    var admin = admins.Find(a => a.id == id);
                    if (admin != null)
                    {
                        Service.Authent.SetCookieHeader(ac, admin.ToToken());
                        goto Redirect;
                    }
                    else { ac.Reply(404); }
                }
                ac.ReplyForm(200, h =>
                {
                    h.TEXT(nameof(id), id);
                    h.PASSWORD(nameof(password), password);
                    h.HIDDEN(nameof(orig), orig);
                    h.CHECKBOX(nameof(remember), remember);
                });

                return;

            Redirect:
                ac.SetHeader("Location", orig);
                ac.Reply(303); // see other (redirect)
            }
        }

        ///
        /// Get nearest shops
        ///
        /// <code>
        /// GET /nearest?pt=x,y
        /// </code>
        ///
        public void nearest(WebActionContext ac)
        {
            NpgsqlPoint pt = ac.Query[nameof(pt)];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Shop.Empty)._("FROM shops WHERE location <-> @1");
                if (dc.Query(p => p.Set(pt)))
                {
                    var shops = dc.ToList<Shop>();
                }
                else
                {
                }
            }
        }


        //
        // management
        //


        [Admin]
        public virtual void mgmt(WebActionContext ac)
        {
            if (Subs != null)
            {
            }
        }


        public async Task report(WebActionContext ac)
        {
            await Task.CompletedTask;
        }
    }
}