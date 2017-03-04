using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : Service<User>
    {
        protected static readonly Client WeiXinClient = new Client("https://sh.api.weixin.qq.com");

        protected readonly WeiXin weixin;

        public AbstService(ServiceContext sc) : base(sc)
        {
            weixin = JsonUtility.FileToObject<WeiXin>(sc.GetFilePath("$weixin.json"));
        }

        protected override async Task Authenticate(ActionContext ac)
        {
            // check cookie token
            string toktext;
            if (ac.Cookies.TryGetValue("Bearer", out toktext))
            {
                ac.Token = Decrypt(toktext);
                return;
            }

            // signon process
            //

            User tok = null;
            string ua = ac.Header("User-Agent");
            if (ua != null && ua.Contains("MicroMessenger")) // if request from weixin
            {
                // as normal request if no # 
                string uri = ac.Uri;
                if (!uri.EndsWith("X-SIGNON"))
                {
                    return;
                }

                string code = ac.Query[nameof(code)];

                // get access token by the code
                JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/oauth2/access_token?appid=" + weixin.appid + "&secret=" + weixin.appsecret + "&code=" + code + "&grant_type=authorization_code");

                string access_token = jo[nameof(access_token)];
                string openid = jo[nameof(openid)];

                // get user info
                jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                string nickname = jo[nameof(nickname)];

                // whether a recorded user?
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(openid)))
                    {
                        tok = dc.ToObject<User>();
                    }
                    else // create a temporary user
                    {
                        tok = new User { wx = openid, wxname = nickname };
                    }
                }
            }
            else
            {
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
                        tok = dc.ToObject<User>();
                    }
                }

                // validate
                if (tok == null || !md5.Equals(tok.credential)) { return; }
            }

            // set token success
            ac.Token = tok;
            SetCookies(ac, tok);
        }

        protected override void Challenge(ActionContext ac)
        {
            string ua = ac.Header("User-Agent");
            if (ua != null && ua.Contains("MicroMessenger")) // weixin
            {
                // redirect the user to weixin authorization page
                ac.ReplyRedirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + weixin.appid + "&redirect_uri=" + ac.UriPad + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
            }
            else
            {
                ac.SetHeader("WWW-Authenticate", "Basic realm=\"" + Auth.domain + "\"");
                ac.Reply(401); // unauthorized
            }
        }
    }
}