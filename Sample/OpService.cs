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


        readonly WebAction[] _new;

        public OpService(WebServiceContext sc) : base(sc)
        {
            Create<ShopFolder>("shop");

            Create<UserFolder>("user");

            _new = GetActions(nameof(@new));
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
                Form frm = ac.Query;
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];

                ac.ReplyPage(200, "", main => { main.FORM(null, x => x.INPUT_button()); });
            }
            else // login
            {
                Form frm = await ac.ReadAsync<Form>();
                string id = frm[nameof(id)];
                string password = frm[nameof(password)];
                string orig = frm[nameof(orig)];
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

        public void test(WebActionContext ac)
        {
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
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty)._("FROM shops WHERE location <-> @1");
                if (dc.Query(sql.ToString(), p => p.Set(pt)))
                {
                    var shops = dc.ToArray<Shop>();
                    ac.ReplyPage(200, "", main => { main.FORM(_new, shops); });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
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
        [CheckAdmin]
        public void @default(WebActionContext ac)
        {
            GetUiActions(typeof(CheckAdminAttribute));

            ac.ReplyPage(200, "", x =>
            {
                // x.Form();
            });
        }

        ///
        /// Get shop list.
        ///
        /// <code>
        /// GET /[-page]
        /// </code>
        ///
        public void mgmtz(WebActionContext ac, string arg)
        {
            int page = arg.ToInt();
            const ushort z = ALL ^ BIN;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty, z)._("FROM shops");
                if (dc.Query(sql.ToString(), p => p.Set(20 * page)))
                {
                    var shops = dc.ToArray<Shop>(z);
                    ac.ReplyPage(200, "", main => { main.FORM(_new, shops); });
                }
                else
                {
                    ac.ReplyPage(200, "没有记录", main => { });
                }
            }
        }

        /// Create a new shop
        ///
        /// <code>
        /// GET /new
        /// </code>
        ///
        /// <code>
        /// POST /new
        ///
        /// id=_shopid_&amp;password=_password_&amp;name=_name_
        /// </code>
        ///
        [CheckAdmin]
        public async Task @new(WebActionContext ac)
        {
            if (ac.GET)
            {
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>(); // read form
                using (var dc = Service.NewDbContext())
                {
                    shop.credential = TextUtility.MD5(shop.id + ':' + shop.credential);
                    DbSql sql = new DbSql("INSERT INTO users")._(Shop.Empty)._VALUES_(Shop.Empty)._("");
                    if (dc.Execute(sql.ToString(), p => p.Set(shop)) > 0)
                    {
                        ac.Reply(201); // created
                    }
                    else
                        ac.Reply(500); // internal server error
                }
            }
        }


        [CheckAdmin]
        public virtual void mgmt(WebActionContext ac)
        {
            if (Subs != null)
            {
                ac.ReplyPage(200, "模块管理", a =>
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