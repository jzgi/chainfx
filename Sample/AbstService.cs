using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : Service<User>, IAuthenticateAsync, ICatch
    {
        protected static readonly Client WeiXinClient = new Client("https://api.weixin.qq.com");

        protected readonly WeiXin weixin;

        public AbstService(ServiceContext sc) : base(sc)
        {
            weixin = JsonUtility.FileToObject<WeiXin>(sc.GetFilePath("$weixin.json"));
        }

        public async Task AuthenticateAsync(ActionContext ac, bool e)
        {
            // check cookie token
            ERR("AuthenticateAsync() --------------");
            ERR("URI: " + ac.Uri);

            string token;
            if (ac.Cookies.TryGetValue("Bearer", out token))
            {
                ac.Principal = Decrypt(token);
                ERR("got cookie principal ok ");
                return;
            }

            User prin = null;
            if (ac.ByWeiXin) // if from weixin
            {
                ERR("From WeiXin");
                string code = ac.Query[nameof(code)];
                if (code == null)
                {
                    ERR("No code, exist ------------AuthenticateAsync");
                    return;
                }

                // get access token by the code
                ERR("to get access_token from weixin ... ");

                JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/oauth2/access_token?appid=" + weixin.appid + "&secret=" + weixin.appsecret + "&code=" + code + "&grant_type=authorization_code");
                if (jo == null)
                {
                    ERR("no return json, exit: -----------AuthenticateAsync");
                    return;
                }

                string access_token = jo[nameof(access_token)];
                if (access_token == null)
                {
                    string errmsg = jo[nameof(errmsg)];
                    ERR("err: " + errmsg);
                    ac.Give(403); // forbidden
                    return;
                }
                string openid = jo[nameof(openid)];

                ERR("access_token: " + access_token);
                ERR("openid: " + openid);

                // whether a recorded user?
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Projection.SECRET);
                    }
                }

                if (prin == null)
                {
                    // get user info
                    ERR("to get userinfo from weixin ... ");

                    jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                    string nickname = jo[nameof(nickname)];
                    string province = jo[nameof(province)];

                    ERR("nickname: " + nickname);
                    prin = new User { city = openid, nickname = nickname, temp = true };
                }
            }
            else
            {
                ERR("From NOT WeiXin");

                string authorization = ac.Header("Authorization");
                if (authorization == null || !authorization.StartsWith("Basic ")) { return; }

                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(authorization.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string id = orig.Substring(0, colon);
                string password = orig.Substring(colon + 1);
                string md5 = TextUtility.MD5(id + ':' + password);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(id)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Projection.SECRET);
                    }
                }

                // validate
                if (prin == null || !md5.Equals(prin.credential)) { return; }
            }

            // set token success
            ac.Principal = prin;
            SetBearerCookie(ac, prin);

            ERR("principal and cookie being set...");
            ERR("successful exist ------------AuthenticateAsync");
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            ERR("GiveChallenge() --------------");
            ERR("URI: " + ac.Uri);

            // weixin authorization challenge
            if (ac.ByWeiXin) // weixin
            {
                // redirect the user to weixin authorization page
                string redirect_url = System.Net.WebUtility.UrlEncode(weixin.addr + ac.Uri);
                ac.GiveRedirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + weixin.appid + "&redirect_uri=" + redirect_url + "&response_type=code&scope=snsapi_userinfo&state=1#wechat_redirect");
            }
            else // challenge BASIC scheme
            {
                ac.SetHeader("WWW-Authenticate", "Basic realm=\"" + Auth.domain + "\"");
                ac.Give(401); // unauthorized
            }
        }
    }
}