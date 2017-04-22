using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    public class ShopService : Service<User>, IAuthenticateAsync, ICatch
    {
        // the timer for triggering periodically obtaining access_token from weixin
        readonly Timer timer;

        volatile string access_token;

        public ShopService(ServiceContext sc) : base(sc)
        {
            Create<PubShopWork>("pub"); // public

            Create<MyUserWork>("my"); // personal

            Create<OprShopWork>("opr"); // shop operator

            Create<DvrShopWork>("dvr"); // shop deliverer

            Create<MgrCityWork>("mgr"); // local manager

            Create<AdmWork>("adm"); // administrator

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

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
        {
            string token;
            if (ac.Cookies.TryGetValue("Token", out token))
            {
                ac.Principal = Decrypt(token);
                return true;
            }

            User prin = null;
            string state = ac.Query[nameof(state)];
            if (WXAUTH.Equals(state)) // if weixin auth
            {
                string code = ac.Query[nameof(code)];
                if (code == null) return false;
                var accessor = await GetAccessorAsync(code);
                if (accessor.access_token == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(accessor.openid)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Projection.SECRET);
                        prin.stored = true;
                    }
                }
                if (prin == null) // get userinfo remotely
                {
                    prin = await GetUserInfoAsync(accessor.access_token, accessor.openid);
                }
            }
            else if (ac.ByBrowse)
            {
                string authorization = ac.Header("Authorization");
                if (authorization == null || !authorization.StartsWith("Basic "))
                {
                    return true;
                }

                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(authorization.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string id = orig.Substring(0, colon);
                string credential = StrUtility.MD5(orig);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE tel = @1", (p) => p.Set(id)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Projection.SECRET);
                    }
                }
                // validate
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return false;
                }
            }
            if (prin != null)
            {
                // set token success
                ac.Principal = prin;
                ac.SetTokenCookie(prin);
            }
            return true;
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            if (e is AuthorizeException)
            {
                if (ac.Principal == null)
                {
                    // weixin authorization challenge
                    if (ac.ByWeiXin) // weixin
                    {
                        ac.GiveRedirectWeiXinAuthorize();
                    }
                    else // challenge BASIC scheme
                    {
                        ac.SetHeader("WWW-Authenticate", "Basic realm=\"" + Auth.domain + "\"");
                        ac.Give(401); // unauthorized
                    }
                }
                else
                {
                    ac.Give(403); // forbidden
                }
            }
            else
            {
                ac.Give(500, e.Message);
            }
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
            string appid = xe.Child(nameof(appid));
            string mch_id = xe.Child(nameof(mch_id));
            string openid = xe.Child(nameof(openid));
            string nonce_str = xe.Child(nameof(nonce_str));
            string sign = xe.Child(nameof(sign));
            string result_code = xe.Child(nameof(result_code));

            string bank_type = xe.Child(nameof(bank_type));
            string total_fee = xe.Child(nameof(total_fee)); // 订单总金额单位分
            string cash_fee = xe.Child(nameof(cash_fee)); // 支付金额单位分
            string transaction_id = xe.Child(nameof(transaction_id)); // 微信支付订单号
            string out_trade_no = xe.Child(nameof(out_trade_no)); // 商户订单号
            string time_end = xe.Child(nameof(time_end)); // 支付完成时间
        }
    }
}