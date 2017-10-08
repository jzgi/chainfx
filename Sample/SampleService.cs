using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    /// <summary>
    /// The care service includes the gospel and the health provision.
    /// </summary>
    public class SampleService : Service<User>, IAuthenticateAsync, ICatch
    {
        readonly Map<string, City> cities;

        public SampleService(ServiceContext sc) : base(sc)
        {
            Create<PubShopWork>("shop"); // shopping

            Create<MyWork>("my"); // personal

            Create<OprWork>("opr"); // shop operator

            Create<AdmWork>("adm"); // administrator

            cities = DataInputUtility.FileToMap<string, City>(sc.GetFilePath("$map.json"), o => o.name);
        }

        public Map<string, City> Cities => cities;

        public City LocateCity(double x, double y)
        {
            foreach (var pair in cities)
            {
                var city = pair.Value;
                if (city.Contains(x, y)) return city;
            }
            return null;
        }

        public void @default(ActionContext ac)
        {
            Lesson[] lessons = null;
            using (var dc = NewDbContext())
            {
                if (dc.Query("SELECT * FROM lessons"))
                {
                    lessons = dc.ToArray<Lesson>(-1 ^ User.CREDENTIAL);
                }
            }
            ac.GivePage(200, m =>
            {
                if (lessons != null)
                {
                    for (int i = 0; i < lessons.Length; i++)
                    {
                        var lesson = lessons[i];
                        m.T("<div class=\"card\">");
                        m.T("<embed src=\"http://player.youku.com/player.php/sid/").T(lesson.refid).T("/v.swf\" allowFullScreen=\"true\" quality=\"high\" width=\"480\" height=\"400\" align=\"middle\" allowScriptAccess=\"always\" type=\"application/x-shockwave-flash\"></embed>");
                        m.T("</div>");
                    }
                }
            });
        }

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
        {
            // if principal already in cookie
            string token;
            if (ac.Cookies.TryGetValue("Token", out token))
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
                ( _, string openid) = await GetAccessorAsync(code);
                if (openid == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", p => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(-1 ^ User.CREDENTIAL);
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
                ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
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
                        ac.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        ac.Give(401); // unauthorized
                    }
                }
                else
                {
                    ac.GivePage(403, m => { m.CALLOUT("您目前没有访问权限!"); });
                }
            }
            else
            {
                ac.Give(500, e.Message);
            }
        }

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task notify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            long orderid;
            decimal cash;
            if (Notified(xe, out orderid, out cash))
            {
                using (var dc = NewDbContext())
                {
                    var shopid = (string) dc.Scalar("UPDATE orders SET cash = @1, accepted = localtimestamp, status = " + Order.ACCEPTED + " WHERE id = @2 AND status < " + Order.ACCEPTED + " RETURNING shopid", (p) => p.Set(cash).Set(orderid));
                    if (shopid != null) // try to send a notification to the operator
                    {
                        var oprwx = (string) dc.Scalar("SELECT oprwx FROM shops WHERE id = @1", p => p.Set(shopid));
                        if (oprwx != null)
                        {
                            await PostSendAsync(oprwx, "【收款通知】单号：" + orderid + "，金额：" + cash + "元");
                        }
                    }
                }
                // return xml to WCPay server
                XmlContent x = new XmlContent(true, 1024);
                x.ELEM("xml", null, () =>
                {
                    x.ELEM("return_code", "SUCCESS");
                    x.ELEM("return_msg", "OK");
                });
                ac.Give(200, x);
            }
            else
            {
                ac.Give(400);
            }
        }
    }
}