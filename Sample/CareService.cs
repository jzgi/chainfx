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
    public class CareService : Service<User>, IAuthenticateAsync, ICatch
    {
        readonly Map<string, City> cities;

        public CareService(ServiceContext sc) : base(sc)
        {
            Create<PubShopWork>("shop"); // shopping

            Create<MyWork>("my"); // personal

            Create<OprWork>("opr"); // shop operator

            Create<AdmWork>("adm"); // administrator

            using (var dc = NewDbContext())
            {
                if (dc.Query("SELECT * FROM cities"))
                {
                    cities = dc.ToMap<string, City>(x => x.name);
                }
            }
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
                    lessons = dc.ToArray<Lesson>(0xffff ^ User.CREDENTIAL);
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
                if (code == null)
                {
                    return false;
                }
                (string access_token, string openid) = await GetAccessorAsync(code);
                if (access_token == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", p => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(0xffff ^ User.CREDENTIAL);
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
                string id = orig.Substring(0, colon);
                string credential = StrUtility.MD5(orig);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE id = @1", p => p.Set(id)))
                    {
                        prin = dc.ToObject<User>(0xffff);
                    }
                }
                // validate
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return true;
                }
            }

            if (prin != null)
            {
                // set token success
                ac.Principal = prin;
                ac.SetTokenCookie(prin, 0xffff ^ User.CREDENTIAL);
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
                string mgrwx = null;
                using (var dc = NewDbContext())
                {
                    var shopid = (string) dc.Scalar("UPDATE orders SET cash = @1, accepted = localtimestamp, status = @2 WHERE id = @3 AND status <= @2 RETURNING shopid", (p) => p.Set(cash).Set(Order.ACCEPTED).Set(orderid));
                    if (shopid != null)
                    {
                        mgrwx = (string) dc.Scalar("SELECT mgrwx FROM shops WHERE id = @1", p => p.Set(shopid));
                    }
                }
                // return xml
                XmlContent cont = new XmlContent(true, 1024);
                cont.ELEM("xml", null, () =>
                {
                    cont.ELEM("return_code", "SUCCESS");
                    cont.ELEM("return_msg", "OK");
                });

                if (mgrwx != null)
                {
                    await PostSendAsync(mgrwx, "【买家付款】订单编号：" + orderid + "，金额：" + cash + "元");
                }

                ac.Give(200, cont);
            }
            else
            {
                ac.Give(400);
            }
        }
    }
}