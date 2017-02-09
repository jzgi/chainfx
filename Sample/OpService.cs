using System.Threading.Tasks;
using Greatbone.Core;
using NpgsqlTypes;
using static Greatbone.Core.Projection;

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

            Create<OrderFolder>("order");
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

        ///
        /// Get the singon form or perform a signon action.
        ///
        /// <code>
        /// GET /signon[id=_id_&amp;password=_password_&amp;orig=_orig_]
        /// </code>
        ///
        /// <code>
        /// POST /signon
        ///  
        /// id=_id_&amp;password=_password_[&amp;orig=_orig_]
        /// </code>
        ///
        public async Task signon(WebActionContext ac)
        {
            if (ac.GET) // return the login form
            {
                Form q = ac.Query;
                string id = q[nameof(id)];
                string password = q[nameof(password)];
                string orig = q[nameof(orig)];

                ac.ReplyPage(200, main => { main.FORM(null, x => x.INPUT_button()); });
            }
            else // login
            {
                Form f = await ac.ReadAsync<Form>();
                string id = f[nameof(id)];
                string password = f[nameof(password)];
                string orig = f[nameof(orig)];
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
                {
                    ac.Reply(400);
                    return; // bad request
                }
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM shops WHERE id = @1", (p) => p.Set(id)))
                    {
                        var tok = dc.ToObject<Token>();
                        string credential = TextUtility.MD5(id + ':' + password);
                        if (credential.Equals(tok.role))
                        {
                            // set cookie
                            string tokstr = Service.Authent.Encrypt(tok);
                            ac.SetHeader("Set-Cookie", tokstr);
                            ac.SetHeader("Location", "");
                            ac.Reply(303); // see other (redirect)
                        }
                        else
                        {
                            ac.Reply(400);
                        }
                    }
                    else
                    {
                        ac.Reply(404);
                    }
                }
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
                    var shops = dc.ToArray<Shop>();
                    ac.ReplyPage(200, main => { main.FORM_grid(null, shops); });
                }
                else
                {
                    ac.ReplyPage(200, main => { });
                }
            }
        }

        #region MANAGEMENT

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        [Admin]
        public void @default(WebActionContext ac)
        {
            GetUiActions(typeof(AdminAttribute));

            ac.ReplyPage(200, x =>
            {
                // x.Form();
            });
        }


        [Admin]
        public virtual void mgmt(WebActionContext ac)
        {
            if (Subs != null)
            {
                ac.ReplyPage(200, a =>
                {
                    for (int i = 0; i < Subs.Count; i++)
                    {
                        WebFolder child = Subs[i];
                    }
                },
                true);
            }
        }


        public async Task report(WebActionContext ac)
        {
            await Task.CompletedTask;
        }

        #endregion
    }
}