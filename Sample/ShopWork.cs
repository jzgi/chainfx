using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.GospelUtility;
using static System.Data.IsolationLevel;
using static Greatbone.Sample.Order;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    public abstract class ShopWork<V> : Work where V : ShopVarWork
    {
        protected ShopWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Shop) obj).id);
        }
    }

    public class PubShopWork : ShopWork<PubShopVarWork>
    {
        public PubShopWork(WorkConfig cfg) : base(cfg)
        {
        }

        /// <summary>
        /// Returns a home page pertaining to a related city
        /// </summary>
//        [City]
        [User] // we are forced to put check here because  weixin auth does't work in iframe
        public void @default(WebContext ac)
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
        public async Task paynotify(WebContext ac)
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
            using (var dc = NewDbContext(ReadCommitted))
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

    [Ui("网点")]
    public class AdmShopWork : ShopWork<AdmShopVarWork>
    {
        public AdmShopWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac)
        {
            using (var dc = NewDbContext())
            {
                dc.Query(dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops ORDER BY id"));
                ac.GiveBoardPage(200, dc.ToArray<Shop>(), (h, o) =>
                {
                    h.CAPTION_().T(o.name).T(" / ").T(o.id)._CAPTION();
                    h.FIELD_("地址").T(o.city)._T(o.addr)._FIELD();
                    h.FIELD_("坐标").T(o.x)._T(o.y)._FIELD();
                    h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext ac)
        {
            const byte proj = Shop.ADM;
            if (ac.GET)
            {
                var o = new Shop {city = City.All[0].name};
                o.Read(ac.Query, proj);
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "编号", max: 4, min: 4, required: true);
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.SELECT(nameof(o.city), o.city, City.All, "城市", refresh: true);
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度", max: 20, box: 6).NUMBER(nameof(o.x), o.x, "纬度", max: 20, box: 6);
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Execute(dc.Sql("INSERT INTO shops")._(Shop.Empty, proj)._VALUES_(Shop.Empty, proj), p => o.Write(p, proj));
                }
                ac.GivePane(200); // created
            }
        }
    }
}