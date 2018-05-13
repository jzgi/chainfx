using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone;
using static Samp.SampUtility;
using static Samp.WeiXinUtility;

namespace Samp
{
    /// <summary>
    /// The sample service includes the gospel and the health provision.
    /// </summary>
    public class SampService : Service<User>, IAuthenticateAsync
    {
        public SampService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<SampVarWork, string>(obj => ((Org) obj).id);

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

            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE status > 0 ORDER BY orgid, name");
                        return dc.Query<(string, string), Item>(proj: 0xff);
                    }
                }, 3600 * 8
            );
        }

        public async Task<bool> AuthenticateAsync(WebContext wc)
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

        public async Task @catch(WebContext wc, int cmd)
        {
            if (cmd == 1) // handle form submission
            {
                var prin = (User) wc.Principal;
                var f = await wc.ReadAsync<Form>();
                string name = f[nameof(name)];
                string tel = f[nameof(tel)];
                string url = f[nameof(url)];
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<User>("INSERT INTO users (name, wx, tel) VALUES (@1, @2, @3) RETURNING *", p => p.Set(name).Set(prin.wx).Set(tel));
                    wc.SetTokenCookie(o, 0xff ^ User.CREDENTIAL);
                }
                wc.GiveRedirect(url);
            }
            else if (wc.Except is AuthorizeException authex)
            {
                if (authex.NoPrincipal)
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
                else if (authex.NullResult)
                {
                    string url = wc.Path;
                    wc.GivePage(200, h =>
                    {
                        h.FORM_();
                        string name = null;
                        string tel = null;
                        h.FIELDSET_("您还没有注册，请填写用户基本信息");
                        h.TEXT(nameof(name), name, label: "姓名", tip: "您本人的姓名", max: 4, min: 2, required: true);
                        h.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, required: true);
                        h._FIELDSET();
                        h.HIDDEN(nameof(url), url);
                        h.ACTIONBAR_().BUTTON("/catch", 1, "确定")._ACTIONBAR();
                        h._FORM();
                    }, title: "注册新帐号");
                }
                else // IsNotAllowed
                {
                    wc.GivePage(403, h => { h.ALERT("您要使用的功能需要管理员授权。"); }, title: "没有访问权限");
                }
            }
            else
            {
                wc.Give(500, wc.Except.Message);
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
                using (var dc = NewDbContext())
                {
                    h.ALERT("　　这里所报告的都是客观存在的事实真相，且假以良知、耐心和洞察力，您就一定能像我们一样认知到这关于生命和敬虔的奥秘。");

                    h.GRID(lessons, o =>
                    {
                        // h.T("<div class=\"uk-inline\">");
                        h.T("<video class=\"uk-width-1-1\" controls playsinline uk-video src=\"http://aliyun.com/").T(o.zh).T("\" type=\"video/mp4\">");
                        h.T("</video>");
                        h.T("<div>").T(o.zh).T("</div>");
                        // h.T("</div>");
                    });
                }
            }, true, 3600, "《生命的奥秘》系列");
        }

        /// Returns a home page pertaining to a related city
        /// We are forced to put auth check here because weixin auth does't work in iframe
//        [CityId]
        [User(false)]
        public void list(WebContext wc)
        {
            string cityid = wc.Query[nameof(cityid)];
            if (string.IsNullOrEmpty(cityid))
            {
                cityid = City.All?[0].id;
            }

            var cityorgs = Obtain<Map<string, Org>>().FindGroup(cityid);
            var items = Obtain<Map<(string, string), Item>>();

            wc.GivePage(200, h =>
                {
                    h.TOPBAR_().SELECT(nameof(cityid), cityid, City.All, refresh: true)._TOPBAR();
                    h.BOARD(cityorgs, o =>
                        {
                            h.T("<section class=\"uk-card-header\">");
                            h.T("<h4>").T(o.name).T("</h4>");
                            if (o.oprtel != null)
                            {
                                //                                h.BADGE_LINK( "/a/", "commenting");
//                                h.BADGE_LINK("tel:" + o.oprtel + "#mp.weixin.qq.com", "receiver");
                            }
                            h.P(o.descr, "简介");
                            h.P_("地址").T(o.addr).T(" ").A_POI(o.x, o.y, o.name, o.addr)._P();

                            h.T("</section>");
                            var orgitems = items.FindGroup((o.id, null));
                            h.LIST(orgitems, m =>
                            {
                                h.ICO_(w: 0x13, css: "uk-padding-small").T("/").T(m.orgid).T("/").T(m.name).T("/icon")._ICO();
                                h.COL_(0x23, css: "uk-padding-small");
                                h.T("<h4>").T(m.name).T("</h4>");
                                h.P(m.descr);
                                h.ROW_();
                                h.P_(w: 0x23).T("<em>¥").T(m.price).T("</em>／").T(m.unit)._P();
                                h.TOOL(nameof(SampItemVarWork.buy));
                                h._ROW();
                                h._COL();
                            }, "uk-card-body uk-padding-remove");

                            h.TOOLPAD();
                        }
                        , article: "uk-card-primary"
                    );
                }, true, 60
            );
        }

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task paynotify(WebContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            if (!OnNotified(xe, out var trade_no, out var cash))
            {
                ac.Give(400);
                return;
            }

            var orgs = Obtain<Map<string, Org>>();
            var (orderid, _) = trade_no.To2Ints();

            string orgid, custname, custaddr;
            using (var dc = NewDbContext())
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = 1 WHERE id = @2 AND status = 0 RETURNING orgid, custname, custaddr", (p) => p.Set(cash).Set(orderid)))
                {
                    return; // WCPay may send notification more than once
                }
                dc.Let(out orgid).Let(out custname).Let(out custaddr);
            }

            // send messages
            var towx = orgs[orgid].oprwx;
            if (towx != null)
            {
                await PostSendAsync(towx, "订单收款", ("¥" + cash + " " + custname + " " + custaddr), NETADDR + "/opr//newo/");
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