using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : Service<Token>
    {
        protected static readonly Client WeiXinClient = new Client("https://sh.api.weixin.qq.com");

        internal readonly Admin[] admins;

        protected readonly WeiXin weixin;

        public AbstService(ServiceContext sc) : base(sc)
        {
            admins = JsonUtility.FileToArray<Admin>(sc.GetFilePath("$admins.json"));

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

            IPrincipal prin = null;
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

                prin = new User
                {
                    wx = openid,
                    wxname = nickname
                };
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
                string credential = TextUtility.MD5(id + ':' + password);
                if (id.Length == 6 && char.IsDigit(id[0])) // is shop id
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM shops WHERE id = @1", (p) => p.Set(id)))
                        {
                            prin = dc.ToObject<Shop>();
                        }
                    }
                }
                else if (id.Length == 11 && char.IsDigit(id[0]))
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(id)))
                        {
                            prin = dc.ToObject<User>();
                        }
                    }
                }
                else // is admin id
                {
                    prin = admins.Find(a => a.id == id && credential.Equals(a.credential));
                }

                // validate
                if (prin == null || !credential.Equals(prin.Credential)) { return; }
            }

            // set token success
            Token tok = prin.ToToken();
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