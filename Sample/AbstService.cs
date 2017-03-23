using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : Service<User>, IAuthenticateAsync, ICatch
    {
        const string WXAUTH = "wxauth";

        protected static readonly Client WeiXinClient = new Client("https://api.weixin.qq.com");

        // weixin config json
        protected readonly WeiXin weixin;

        public AbstService(ServiceContext sc) : base(sc)
        {
            weixin = JsonUtility.FileToObject<WeiXin>(sc.GetFilePath("$weixin.json"));
        }

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
        {
            ERR("---- AuthenticateAsync() -----");
            ERR("URI: " + ac.Uri);

            string token;
            if (ac.Cookies.TryGetValue("Bearer", out token))
            {
                ERR("-------- Bearer Cookie");
                ac.Principal = Decrypt(token);
                return true;
            }

            User prin = null;
            string state = ac.Query[nameof(state)];
            if (WXAUTH.Equals(state)) // if weixin auth
            {
                ERR("-------- WXAUTH");
                // get access token by the code parameter value
                string code = ac.Query[nameof(code)];
                JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/oauth2/access_token?appid=" + weixin.appid + "&secret=" + weixin.appsecret + "&code=" + code + "&grant_type=authorization_code");
                if (jo == null) { return false; }

                string access_token = jo[nameof(access_token)];
                if (access_token == null)
                {
                    string errmsg = jo[nameof(errmsg)];
                    ERR("err getting access_token: " + errmsg);
                    return false;
                }
                string openid = jo[nameof(openid)];

                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Proj.SECRET);
                        prin.stored = true;
                    }
                }
                if (prin == null) // get userinfo remotely
                {
                    jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                    string nickname = jo[nameof(nickname)];
                    string city = jo[nameof(city)];
                    prin = new User { wx = openid, nickname = nickname, city = city };
                }
            }
            else if (ac.ByBrowse)
            {
                ERR("-------- ByBrowse");
                string authorization = ac.Header("Authorization");
                if (authorization == null || !authorization.StartsWith("Basic ")) { return true; }

                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(authorization.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string id = orig.Substring(0, colon);
                string password = orig.Substring(colon + 1);
                string md5 = TextUtility.MD5(id + ':' + password);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE tel = @1", (p) => p.Set(id)))
                    {
                        prin = dc.ToObject<User>(-1 ^ Proj.SECRET);
                    }
                }
                // validate
                if (prin == null || !md5.Equals(prin.credential)) { return false; }
            }
            if (prin != null)
            {
                // set token success
                ac.Principal = prin;
                SetBearerCookie(ac, prin);
            }
            return true;
        }

        public virtual void Catch(Exception e, ActionContext ac)
        {
            ERR("---- Catch() -----");
            ERR("URI: " + ac.Uri);
            if (e is AuthorizeException)
            {
                if (ac.Principal == null)
                {
                    ERR("-------- NoToken");
                    // weixin authorization challenge
                    if (ac.ByWeiXin) // weixin
                    {
                        ERR("------------ ByWeiXin");
                        // redirect the user to weixin authorization page
                        string redirect_url = System.Net.WebUtility.UrlEncode(weixin.addr + ac.Uri);
                        ac.GiveRedirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + weixin.appid + "&redirect_uri=" + redirect_url + "&response_type=code&scope=snsapi_userinfo&state=" + WXAUTH + "#wechat_redirect");
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