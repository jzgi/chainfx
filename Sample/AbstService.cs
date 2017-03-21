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

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
        {
            string token;
            if (ac.Cookies.TryGetValue("Bearer", out token))
            {
                ac.Principal = Decrypt(token);
                return true;
            }

            User prin = null;
            if (ac.ByWeiXin) // if from weixin
            {
                string code = ac.Query[nameof(code)];
                if (code == null)
                {
                    return true; // exit normal
                }

                // get access token by the code
                JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/oauth2/access_token?appid=" + weixin.appid + "&secret=" + weixin.appsecret + "&code=" + code + "&grant_type=authorization_code");
                if (jo == null)
                {
                    return false;
                }

                string access_token = jo[nameof(access_token)];
                if (access_token == null)
                {
                    string errmsg = jo[nameof(errmsg)];
                    ERR("err: " + errmsg);
                    return false;
                }
                string openid = jo[nameof(openid)];

                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Projection.SECRET);
                    }
                }
                if (prin == null) // if not an existing user
                {
                    // get user info
                    jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                    string nickname = jo[nameof(nickname)];
                    string city = jo[nameof(city)];
                    prin = new User { wx = openid, nickname = nickname, city = city };
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("INSERT INTO users (wx, nickname, city) VALUES (@1, @2, @3)", (p) => p.Set(openid).Set(nickname).Set(city));
                    }
                }
            }
            else
            {
                string authorization = ac.Header("Authorization");
                if (authorization == null || !authorization.StartsWith("Basic ")) { return false; }

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
                if (prin == null || !md5.Equals(prin.credential)) { return false; }
            }

            // set token success
            ac.Principal = prin;
            SetBearerCookie(ac, prin);
            return true;
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            if (e is AuthorizeException)
            {
                ERR("GiveChallenge() --------------");
                ERR("URI: " + ac.Uri);

                if (((AuthorizeException)e).NoToken)
                {
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
    }
}