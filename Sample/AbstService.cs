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

            // as normal request if no # 
            string uri = ac.Uri;
            if (!uri.EndsWith("#"))
            {
                return;
            }

            // signon process
            //

            string ua = ac.Header("User-Agent");
            if (ua != null && ua.Contains("MicroMessenger")) // if request from weixin
            {
                string code = ac.Query[nameof(code)];

                // get access token by the code
                JObj jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/oauth2/access_token?appid=" + weixin.appid + "&secret=" + weixin.appsecret + "&code=" + code + "&grant_type=authorization_code");

                string access_token = jo[nameof(access_token)];
                string openid = jo[nameof(openid)];

                // get user info
                jo = await WeiXinClient.GetAsync<JObj>(null, "/sns/userinfo?access_token=" + access_token + "&openid=" + openid);
                string nickname = jo[nameof(nickname)];

                User prin = new User
                {
                    wx = openid,
                    wxname = nickname
                };

                Token tok = prin.ToToken();
                ac.Token = tok;
                SetCookies(ac, tok);
            }
            else
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
                    IPrincipal prin = null;
                    if (login.IsShop)
                    {
                        using (var dc = Service.NewDbContext())
                        {
                            if (dc.Query1("SELECT * FROM shops WHERE id = @1", (p) => p.Set(login.id)))
                            {
                                prin = dc.ToObject<Shop>();
                            }
                        }
                    }
                    else if (login.IsUser)
                    {
                        using (var dc = Service.NewDbContext())
                        {
                            if (dc.Query1("SELECT * FROM users WHERE id = @1", (p) => p.Set(login.id)))
                            {
                                prin = dc.ToObject<User>();
                            }
                        }
                    }
                    else // is admin id
                    {
                        prin = admins.Find(a => a.id == login.id && credential.Equals(a.credential));
                    }

                    // validate
                    if (prin != null && credential.Equals(prin.Credential))
                    {
                        SetCookies(ac, prin.ToToken());
                        ac.ReplyRedirect(login.orig);
                        return;
                    }
                    else { ac.Reply(404); }
                    // error
                    ac.ReplyForm(200, login);
                }
            }
        }

        protected override void Challenge(ActionContext ac)
        {
            string ua = ac.Header("User-Agent");
            if (ua != null && ua.Contains("MicroMessenger")) // weixin
            {
                // redirect the user to weixin authorization page
                ac.ReplyRedirect("https://open.weixin.qq.com/connect/oauth2/authorize?appid=" + weixin.appid + "&redirect_uri=" + ac.Uri + "#&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect");
            }
            else
            {
                ac.SetHeader("Location", ac.Uri + "#");
                ac.Reply(303); // see other - redirect to signon url
            }
        }
    }
}