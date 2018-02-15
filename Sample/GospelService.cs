using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.GospelUtility;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    /// <summary>
    /// The sample service includes the gospel and the health provision.
    /// </summary>
    public class GospelService : Service<User>, IAuthenticateAsync
    {
        public GospelService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<GospelVarWork, string>(obj => ((Shop) obj).id); // subshop

            Create<PubShopWork>("shop"); // personal

            Create<MyWork>("my"); // personal

            Create<OprWork>("opr"); // shop operator

            Create<AdmWork>("adm"); // administrator

            Register(delegate
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT DISTINCT lesson FROM slides WHERE status > 0 ORDER BY lesson");
                    Roll<string> roll = new Roll<string>(32);
                    while (dc.Next())
                    {
                        dc.Let(out string v);
                        roll.Add(v);
                    }
                    return roll.ToArray();
                }
            }, 3600 * 8);

            Register(delegate
            {
                using (var dc = NewDbContext())
                {
                    return dc.Query<string, Shop>(dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE status > 0 ORDER BY id"), proj: 0xff);
                }
            }, 3600 * 8);
        }

        public override void OnStart()
        {
            City.All = DataUtility.FileToMap<string, City>(GetFilePath("$cities.json"));
        }

        public override void OnStop()
        {
            City.All = null;
        }

        public async Task<bool> AuthenticateAsync(WebContext ac, bool e)
        {
            // if principal already in cookie
            if (ac.Cookies.TryGetValue("Token", out var token))
            {
                ac.Principal = Decrypt(token);
                return true;
            }

            // resolve principal thru OAuth2 or HTTP-basic
            User prin = null;
            string state = ac.Query[nameof(state)];
            if (WXAUTH.Equals(state)) // if weixin auth
            {
                string code = ac.Query[nameof(code)];
                if (code == null)
                {
                    return false;
                }
                (_, string openid) = await GetAccessorAsync(code);
                if (openid == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", p => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(0xff ^ User.CREDENTIAL);
                    }
                    else
                    {
                        prin = new User {wx = openid}; // create a minimal principal object
                    }
                }
            }
            else if (ac.ByBrowse)
            {
                string h_auth = ac.Header("Authorization");
                if (h_auth == null || !h_auth.StartsWith("Basic "))
                {
                    return true;
                }
                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(h_auth.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string tel = orig.Substring(0, colon);
                string credential = StrUtility.MD5(orig);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE tel = @1", p => p.Set(tel)))
                    {
                        prin = dc.ToObject<User>();
                    }
                }
                // validate
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return true;
                }
            }

            // setup principal and cookie
            if (prin != null)
            {
                // set token success
                ac.Principal = prin;
                ac.SetTokenCookie(prin, 0xff ^ User.CREDENTIAL);
            }
            return true;
        }

        public override void Catch(Exception ex, WebContext wc)
        {
            if (ex is AuthorizeException)
            {
                if (wc.Principal == null)
                {
                    // weixin authorization challenge
                    if (wc.ByWeiXin) // weixin
                    {
                        wc.GiveRedirectWeiXinAuthorize(NETADDR);
                    }
                    else // challenge BASIC scheme
                    {
                        wc.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        wc.Give(401); // unauthorized
                    }
                }
                else
                {
                    wc.GivePane(403, m =>
                    {
                        m.FORM_();
                        m.FIELDSET_("没有访问权限");
                        m.P("您要访问的功能需要经过管理员授权后才能使用。");
                        m._FIELDSET();
                        m._FORM();
                    });
                }
            }
            else
            {
                wc.Give(500, ex.Message);
            }
        }

        /// <summary>
        /// The home page that lists gospel lessons.
        /// </summary>
        public void @default(WebContext ac)
        {
            var lessons = Obtain<string[]>();

            ac.GivePage(200, m =>
            {
                m.T("<h1>关于天国的事实真相</h1>");
                using (var dc = NewDbContext())
                {
                    for (int i = 0; i < lessons?.Length; i++)
                    {
                        m.T("<div class=\"card\">");
                        m.T(lessons[i]);
                        m.T("</div>");
                    }
                }
            });
        }
    }
}