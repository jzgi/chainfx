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
            CreateVar<GospelVarWork, string>(obj => ((Org) obj).id); // subshop

            Create<PubOrgWork>("org"); // personal

            Create<MyWork>("my"); // personal

            Create<OprWork>("opr"); // org operator

            Create<AdmWork>("adm"); // administrator

            City.All = DataUtility.FileToMap<string, City>(GetFilePath("$cities.json"));

            Register(() => DataUtility.FileToArray<Lesson>(GetFilePath("$lessons.json")), 3600 * 8);

            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs WHERE status > 0 ORDER BY id");
                        return dc.Query<string, Org>(proj: 0xff);
                    }
                }, 3600 * 8
            );
        }

        public async Task<bool> AuthenticateAsync(WebContext wc, bool e)
        {
            // if principal already in cookie
            if (wc.Cookies.TryGetValue("Token", out var token))
            {
                wc.Principal = Decrypt(token);
                return true;
            }

            // resolve principal thru OAuth2 or HTTP-basic
            User prin = null;
            string state = wc.Query[nameof(state)];
            if (WXAUTH.Equals(state)) // if weixin auth
            {
                string code = wc.Query[nameof(code)];
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
            else
            {
                string h_auth = wc.Header("Authorization");
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
                wc.Principal = prin;
                wc.SetTokenCookie(prin, 0xff ^ User.CREDENTIAL);
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
                    if (wc.ByWeiXinClient) // weixin
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
        /// The home page that lists gospel episodes.
        /// </summary>
        public void @default(WebContext wc)
        {
            var lessons = Obtain<Lesson[]>();
            wc.GivePage(200, m =>
            {
                m.T("<h1>事实真相</h1>");
                using (var dc = NewDbContext())
                {
                    for (int i = 0; i < lessons?.Length; i++)
                    {
                        m.T("<div class=\"card\">");
                        m.T("</div>");
                    }
                }
            });
        }
    }
}