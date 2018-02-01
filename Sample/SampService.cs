using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;
using static System.Data.IsolationLevel;
using static Greatbone.Samp.Order;
using static Greatbone.Samp.SampUtility;
using static Greatbone.Samp.WeiXinUtility;

namespace Greatbone.Samp
{
    /// <summary>
    /// The sample service includes the gospel and the health provision.
    /// </summary>
    public class SampService : Service<User>, IAuthenticateAsync
    {
        public SampService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<SampVarWork, string>(obj => ((Shop) obj).id); // subshop

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

        public async Task<bool> AuthenticateAsync(ActionContext ac, bool e)
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
                using (var dc = ac.NewDbContext())
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
                using (var dc = ac.NewDbContext())
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

        public override void Catch(Exception ex, ActionContext ac)
        {
            if (ex is AuthorizeException)
            {
                if (ac.Principal == null)
                {
                    // weixin authorization challenge
                    if (ac.ByWeiXin) // weixin
                    {
                        ac.GiveRedirectWeiXinAuthorize(NETADDR);
                    }
                    else // challenge BASIC scheme
                    {
                        ac.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        ac.Give(401); // unauthorized
                    }
                }
                else
                {
                    ac.GivePane(403, m =>
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
                ac.Give(500, ex.Message);
            }
        }

        /// <summary>
        /// The home page that lists gospel lessons.
        /// </summary>
        public void @default(ActionContext ac)
        {
            var lessons = Obtain<string[]>();

            ac.GivePage(200, m =>
            {
                m.T("<h1>关于天国的事实真相</h1>");
                using (var dc = ac.NewDbContext())
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

        /// <summary>
        /// Returns a home page pertaining to a related city
        /// </summary>
//        [City]
        [User] // we are forced to put check here because  weixin auth does't work in iframe
        public void list(ActionContext ac)
        {
            var shops = Obtain<Map<string, Shop>>();

            string city = ac.Query[nameof(city)];
            if (string.IsNullOrEmpty(city))
            {
                city = City.All?[0].name;
            }
            ac.GiveDoc(200, m =>
            {
                m.TOPBAR_().SELECT(nameof(city), city, City.All, refresh: true, box: 0)._TOPBAR();
                m.BOARDVIEW(shops.All(x => x.city == city), (h, o) =>
                {
                    h.CAPTION_().T(o.name)._CAPTION(Shop.Statuses[o.status], o.status == 2);
                    h.ICON(o.id + "/icon", href: o.id + "/", box: 0x14);
                    h.BOX_(0x48);
                    h.P_("地址").T(o.addr).T(" ").A_POI(o.x, o.y, o.name, o.addr)._P();
                    h.P_("派送").T(o.delivery);
                    if (o.areas != null) h.SEP().T("限送").T(o.areas);
                    h._P();
                    h.P(o.schedule, "营业");
                    if (o.off > 0)
                        h.P_("优惠").T(o.min).T("元起订, 每满").T(o.notch).T("元立减").T(o.off).T("元")._P();
                    h._BOX();
                    h.THUMBNAIL(o.id + "/img-1", box: 3).THUMBNAIL(o.id + "/img-2", box: 3).THUMBNAIL(o.id + "/img-3", box: 3).THUMBNAIL(o.id + "/img-4", box: 3);
                    h.TAIL();
                });
            }, true, 60, "粗狼达人 - " + city);
        }

        /// <summary>
        /// WCPay notify, placed here due to non-authentic context.
        /// </summary>
        public async Task paynotify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            if (!Notified(xe, out var trade_no, out var cash))
            {
                ac.Give(400);
                return;
            }
            var shops = Obtain<Map<string, Shop>>();
            var (orderid, _) = trade_no.To2Ints();
            string city, addr;
            string towx = null; // messge to
            using (var dc = ac.NewDbContext(ReadCommitted))
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = " + PAID + " WHERE id = @2 AND status < " + PAID + " RETURNING shopid, city, addr", (p) => p.Set(cash).Set(orderid)))
                {
                    return; // WCPay may send notification more than once
                }
                dc.Let(out string shopid).Let(out city).Let(out addr);
                // retrieve a POS openid
                if (shops[shopid].areas != null)
                {
                    var (a, _) = addr.ToDual(SEPCHAR);
                    towx = (string) dc.Scalar("SELECT wx FROM orders WHERE status = 0 AND shopid = @1 AND typ = 1 AND city = @2 AND addr LIKE @3 LIMIT 1", p => p.Set(shopid).Set(city).Set(a + "%"));
                }
                if (towx == null)
                {
                    towx = shops[shopid].oprwx;
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