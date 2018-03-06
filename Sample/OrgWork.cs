using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.GospelUtility;
using static System.Data.IsolationLevel;
using static Greatbone.Sample.Order;
using static Greatbone.Sample.WeiXinUtility;

namespace Greatbone.Sample
{
    public abstract class OrgWork<V> : Work where V : OrgVarWork
    {
        protected OrgWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Org) obj).id);
        }
    }

    public class PubOrgWork : OrgWork<PubOrgVarWork>
    {
        public PubOrgWork(WorkConfig cfg) : base(cfg)
        {
        }

        /// Returns a home page pertaining to a related city
        /// We are forced to put auth check here because weixin auth does't work in iframe
        [City, User]
        public void @default(WebContext ac)
        {
            var orgs = Obtain<Map<string, Org>>();
            string city = ac.Query[nameof(city)];
            if (string.IsNullOrEmpty(city))
            {
                city = City.All?[0].name;
            }
            ac.GiveDoc(200, m =>
                {
                    m.TOPBAR_().SELECT(nameof(city), city, City.All, refresh: true, width: 0)._TOPBAR();
                    m.GRIDVIEW(orgs.All(x => x.city == city), (h, o) =>
                        {
                            h.CHEAD_().T(o.name)._CHEAD(Org.Statuses[o.status], o.status == 2);

                            h.CBODY_();
                            h.ICON(o.id + "/icon", href: o.id + "/", width: 2);
                            h.BOX_(4);
                            h.P(o.descr, "简介");
                            h.P_("地址").T(o.addr).T(" ").A_POI(o.x, o.y, o.name, o.addr)._P();
                            h._BOX();
                            if (o.off > 0)
                            {
                                h.FIELD_("优惠").T(o.min).T("元起订, 每满").T(o.notch).T("元立减").T(o.off).T("元")._FIELD();
                            }

                            h.THUMBNAIL(o.id + "/img-1", box: 2).THUMBNAIL(o.id + "/img-2", box: 2).THUMBNAIL(o.id + "/img-3", box: 2);
                            h._CBODY();

                            h.CFOOT();
                        }
                    );
                }, true, 60, "粗狼达人 - " + city
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
            using (var dc = NewDbContext(ReadCommitted))
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = " + PAID + " WHERE id = @2 AND status < " + PAID + " RETURNING orgid, city, addr", (p) => p.Set(cash).Set(orderid)))
                {
                    return; // WCPay may send notification more than once
                }
                dc.Let(out string orgid).Let(out city).Let(out addr);
                // retrieve a POS openid
                if (orgs[orgid].areas != null)
                {
                    var (a, _) = addr.ToDual(SEPCHAR);
                    towx = (string) dc.Scalar("SELECT wx FROM orders WHERE status = 0 AND orgid = @1 AND typ = 1 AND city = @2 AND addr LIKE @3 LIMIT 1", p => p.Set(orgid).Set(city).Set(a + "%"));
                }
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

    [Ui("网点")]
    public class AdmOrgWork : OrgWork<AdmOrgVarWork>
    {
        public AdmOrgWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").lst(Org.Empty).T(" FROM orgs ORDER BY id");
                dc.Query();
                ac.GiveBoardPage(200, dc.ToArray<Org>(), (h, o) =>
                {
                    h.CHEAD_().T(o.name).T(" / ").T(o.id)._CHEAD();
                    h.FIELD(o.descr, "简介");
                    h.FIELD_("地址").T(o.city)._T(o.addr)._FIELD();
                    h.FIELD_("坐标").T(o.x)._T(o.y)._FIELD();
                    h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                    //                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext ac)
        {
            const byte proj = Org.ADM;
            if (ac.GET)
            {
                var o = new Org {city = City.All[0].name};
                o.Read(ac.Query, proj);
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "编号", max: 4, min: 4, required: true);
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.TEXTAREA(nameof(o.descr), o.descr, "简介", max: 50, required: true);
                    m.SELECT(nameof(o.city), o.city, City.All, "城市", refresh: true);
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度", max: 20, width: 6).NUMBER(nameof(o.x), o.x, "纬度", max: 20, width: 6);
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Org>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO orgs")._(Org.Empty, proj)._VALUES_(Org.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.GivePane(200); // created
            }
        }
    }
}