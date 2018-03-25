using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Greatbone;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Libuv.Internal.Networking;
using static Core.CoreUtility;
using static Core.WeiXinUtility;

namespace Core
{
    /// <summary>
    /// The sample service includes the gospel and the health provision.
    /// </summary>
    public class CoreService : Service<User>, IAuthenticateAsync
    {
        public CoreService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<CoreVarWork, string>(obj => ((Org) obj).id);

            Create<MyWork>("my"); // personal

            Create<OprWork>("opr"); // org operator

            Create<AdmWork>("adm"); // administrator

            City.All = DataUtility.FileToArray<City>(GetFilePath("$cities.json"));

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


        const string Aliyun = "http://aliyun.com/";

        /// <summary>
        /// The home page that lists gospel lessons.
        /// </summary>
        public void @default(WebContext wc)
        {
            var lessons = Obtain<Lesson[]>();
            wc.GivePage(200, h =>
            {
                h.TOPBAR_()._TOOLBAR("天国近了", false);
                using (var dc = NewDbContext())
                {
                    h.GRIDVIEW(lessons, o =>
                    {
                        h.T("<article class=\"uk-card\">");
                        h.T("<video controls playsinline uk-video=\"automute: true\">");
                        h.T("<source src=\"http://aliyun.com/").T(o.zh).T("\" type=\"video/mp4\">");
                        h.T("</video>");
                        h.T("<div>").T(o.zh).T("</div>");
                        h.T("</article>");
                    });
                }
            }, true, 3600, "劝世福音");
        }

        /// Returns a home page pertaining to a related city
        /// We are forced to put auth check here because weixin auth does't work in iframe
        [CityId, User]
        public void list(WebContext ac)
        {
            string cityid = ac.Query[nameof(cityid)];
            if (string.IsNullOrEmpty(cityid))
            {
                cityid = City.All?[0].id;
            }

            var orgs = Obtain<Map<string, Org>>();
            var shops = orgs.All(x => x.id.StartsWith(cityid));
            Item[] items = null;
            using (var dc = NewDbContext())
            {
                items = dc.Query<Item>("SELECT * FROM items WHERE orgid LIKE @1 AND status > 0 ORDER BY orgid, status", p => p.Set(cityid + "%"));
            }
            ac.GiveDoc(200, h =>
                {
                    h.TOPBAR_().SELECT(nameof(cityid), cityid, City.All, refresh: true, width: 0)._TOPBAR();

                    h.BOARDVIEW(shops,
                        o =>
                        {
                            h.H3(o.name);
                            h.P(o.descr, "简介");
                            h.P_("地址").T(o.addr).T(" ").A_POI(o.x, o.y, o.name, o.addr)._P();
                        },
                        o =>
                        {
                            h.LISTVIEW(items, itm =>
                            {
                                h.ICON("/" + itm.orgid + "/" + itm.name + "/icon", width: 0x13);
                                h.BOX_(0x23);
                                h.T(itm.descr);
                                h._BOX();
                                h.P_().TOOL(nameof(CoreItemVarWork.buy))._P();
                            });
                        }
                        , o => h.TOOLPAD()
                    );
                }, true, 60, "粗狼达人 - " + cityid
            );
        }

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task paynotify(WebContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            if (!Notified(xe, out var trade_no, out var cash))
            {
                ac.Give(400);
                return;
            }
            var orgs = Obtain<Map<string, Org>>();
            var (orderid, _) = trade_no.To2Ints();
            string city, addr;
            string towx = null; // messge to
            using (var dc = NewDbContext(IsolationLevel.ReadCommitted))
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = 1 WHERE id = @2 AND status < 2 RETURNING orgid, city, addr", (p) => p.Set(cash).Set(orderid)))
                {
                    return; // WCPay may send notification more than once
                }
                dc.Let(out string orgid).Let(out city).Let(out addr);
                // retrieve a POS openid
                if (towx == null)
                {
                    towx = orgs[orgid].oprwx;
                }
            }
            // send messages
            if (towx != null)
            {
                await PostSendAsync(towx, "收到新单 No." + orderid, "地址: " + city + addr + "  付款: ¥" + cash, NETADDR + "/opr//newly/");
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
    }
}