using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Greatbone.Core;
using static System.Data.IsolationLevel;
using static Greatbone.Samp.Order;
using static Greatbone.Samp.Program;
using static Greatbone.Samp.WeiXinUtility;

namespace Greatbone.Samp
{
    /// <summary>
    /// A before filter that ensures city is resolved and given in the URL.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CityAttribute : Attribute, IBefore
    {
        public bool Do(ActionContext ac)
        {
            string city = ac.Query[nameof(city)];
            if (city == null) // absent
            {
                User prin = (User) ac.Principal;
                if (prin?.city != null) // use those in principal
                {
                    ac.AddParam(nameof(prin.city), prin.city);
                    return true;
                }

                // give out a geolocation page
                //
                HtmlContent h = new HtmlContent(ac, true);
                // geolocator page
                h.T("<html><head><script>");
                //                h.T("var cities = ").JSON(City.All).T(";");
                h.T("var city = cities[0].name;");
                h.T("navigator.geolocation.getCurrentPosition(function(p) {");
                h.T("var x=p.coords.longitude; var y=p.coords.latitude;");
                h.T("for (var i = 0; i < city.length; i++) {");
                h.T("var c = cities[i];");
                h.T("if (c.x1 < x && x < c.x2 && c.y1 < y && y < c.y2) {");
                h.T("city=c.name;");
                h.T("break;");
                h.T("}");
                h.T("}");
                h.T("window.location.href = '/shop/?city=' + city;");
                h.T("},");
                h.T("function(e) {");
                h.T("window.location.href = '/shop/?city=' + city;");
                h.T("}, {enableHighAccuracy: true,timeout: 1000,maximumAge: 0}");
                h.T(")");
                h.T("</script></head></html>");
                ac.Give(200, h);
                return false;
            }
            return true;
        }
    }

    /// <summary>
    /// The sample service includes the gospel and the health provision.
    /// </summary>
    public class SampService : Service<User>, IAuthenticateAsync
    {
        public readonly Roll<string, Shop> ShopRoll;

        public SampService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<PubShopVarWork, string>(obj => ((Shop) obj).id); // subshop

            Create<MyWork>("my"); // personal

            Create<OprWork>("opr"); // shop operator

            Create<AdmWork>("adm"); // administrator

            ShopRoll = new Roll<string, Shop>(delegate
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT * FROM shops WHERE status > 0 ORDER BY id");
                    return dc.ToMap<string, Shop>(x => x.id, -1);
                }
            }, 60 * 30);
        }

        public override void OnStart()
        {
            City.All = DataInputUtility.FileToMap<string, City>(GetFilePath("$cities.json"), o => o.name);
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
                ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
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
                        m.BOX_().T("您要访问的功能需要经过管理员授权后才能使用。")._BOX();
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
            ac.GivePage(200, m =>
            {
                using (var dc = ac.NewDbContext())
                {
                    var lessons = dc.Query<Lesson>("SELECT * FROM lessons");
                    for (int i = 0; i < lessons?.Length; i++)
                    {
                        var lesson = lessons[i];
                        m.T("<div class=\"card\">");
                        m.T("<embed src=\"http://player.youku.com/player.php/sid/").T(lesson.refid).T("/v.swf\" allowFullScreen=\"true\" quality=\"high\" width=\"480\" height=\"400\" align=\"middle\" allowScriptAccess=\"always\" type=\"application/x-shockwave-flash\"></embed>");
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
            string city = ac.Query[nameof(city)];
            if (string.IsNullOrEmpty(city))
            {
                city = City.All?[0].name;
            }
            ac.GiveDoc(200, m =>
            {
                m.TOPBAR_().SELECT(nameof(city), city, City.All, refresh: true, box: 0)._TOPBAR();
                m.BOARDVIEW(ShopRoll.All(x => x.city == city), (h, o) =>
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
            var (orderid, _) = trade_no.To2Ints();
            string city, addr;
            List<string> wxlst = new List<string>(4);
            using (var dc = ac.NewDbContext(ReadCommitted))
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = " + PAID + " WHERE id = @2 AND status < " + PAID + " RETURNING shopid, city, addr, items", (p) => p.Set(cash).Set(orderid)))
                {
                    // WCPay may send notification more than once
                    return;
                }
                dc.Let(out string shopid).Let(out city).Let(out addr).Let(out OrderItem[] items);
                for (int i = 0; i < items?.Length; i++) // update in stock
                {
                    var o = items[i];
                    dc.Execute("UPDATE items SET stock = stock - @1 WHERE shopid = @2 AND name = @3", p => p.Set(o.qty).Set(shopid).Set(o.name));
                }
                
                if (ShopRoll[shopid].areas != null) // retrieve POS openid(s)
                {
                    var (a, _, _) = addr.ToTriple(SEPCHAR);
                    dc.Query("SELECT wx FROM orders WHERE status = 0 AND shopid = @1 AND typ = 1 AND city = @2 AND addr LIKE @3", p => p.Set(shopid).Set(city).Set(a + "%"));
                    while (dc.Next())
                    {
                        dc.Let(out string wx);
                        wxlst.Add(wx);
                    }
                }
                if (wxlst.Count == 0)
                {
                    var oprwx = ShopRoll[shopid].oprwx;
                    if (oprwx != null) wxlst.Add(oprwx);
                }
            }
            // send messages
            foreach (var wx in wxlst)
            {
                await PostSendAsync(wx, "收到新单", "单号: " + orderid + "  地址: " + city + addr + "  付款: ¥" + cash, NETADDR + "/opr//newly/");
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