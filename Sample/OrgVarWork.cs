using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.GospelUtility;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    public abstract class OrgVarWork : Work
    {
        protected OrgVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext ac)
        {
            string orgid = ac[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM orgs WHERE id = @1", p => p.Set(orgid)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204, @public: true, maxage: 3600); // no content 
                    else ac.Give(200, new StaticContent(byteas), @public: true, maxage: 3600);
                }
                else ac.Give(404, @public: true, maxage: 3600); // not found
            }
        }

        public void img(WebContext ac, int ordinal)
        {
            string orgid = ac[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM orgs WHERE id = @1", p => p.Set(orgid)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204, @public: true, maxage: 3600); // no content 
                    else ac.Give(200, new StaticContent(byteas), @public: true, maxage: 3600);
                }
                else ac.Give(404, @public: true, maxage: 3600); // not found
            }
        }
    }

    [User]
    public class PubOrgVarWork : OrgVarWork, IShopVar
    {
        public PubOrgVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<SampVarVarWork, string>(obj => ((Item) obj).name);
        }

        [Ui("进入该网点"), Tool(Anchor)]
        public void @default(WebContext ac)
        {
            string orgid = ac[this];
            var shop = Obtain<Map<string, Org>>()[orgid];
            if (shop == null)
            {
                ac.Give(404, @public: true, maxage: 3600);
                return;
            }
            ac.Register(shop);

            using (var dc = NewDbContext())
            {
                var items = dc.Query<Item>(dc.Sql("SELECT ").columnlst(Item.Empty, 0xff).T(", (img1 IS NOT NULL AND img2 IS NOT NULL AND img3 IS NOT NULL AND img4 IS NOT NULL) AS imgg FROM items WHERE orgid = @1 AND status > 0 ORDER BY status DESC"), p => p.Set(orgid), 0xff);
                ac.GiveDoc(200, m =>
                {
                    m.TOPBAR_().A_DROPDOWN_("正在" + Org.Statuses[shop.status]);
                    m.BOX_(0x4c);
                    m.P(shop.schedule, "营业");
                    m.P_("派送").T(shop.delivery);
                    if (shop.areas != null) m.SEP().T("限送").T(shop.areas);
                    m._P();
                    if (shop.off > 0) m.P_("优惠").T(shop.min).T("元起订，每满").T(shop.notch).T("元立减").T(shop.off).T("元")._P();
                    m._BOX();
                    m.QRCODE(NETADDR + ac.Uri, box: 0x15);
                    if (shop.oprtel != null) m.FIELD_(box: 0x17).A("&#128222; 联系客服", "tel:" + shop.oprtel + "#mp.weixin.qq.com", true)._FIELD();
                    m._A_DROPDOWN()._TOPBAR();

                    if (items == null) return;
                    m.BOARDVIEW(items, (h, o) =>
                    {
                        h.CAPTION(o.name);
                        h.ICON((o.name) + "/icon", box: 4);
                        h.BOX_(0x48).P(o.descr, "特色").P(o.stock, "可供", o.unit).P(o.price, fix: "¥", tag: "em")._BOX();
                        if (o.imgg)
                        {
                            h.THUMBNAIL(o.name + "/img-1", box: 3).THUMBNAIL(o.name + "/img-2", box: 3).THUMBNAIL(o.name + "/img-3", box: 3).THUMBNAIL(o.name + "/img-4", box: 3);
                        }
                        h.TAIL();
                        // adjust item availability
                        if (shop.status == 0) o.stock = 0;
                    });
                }, true, 60, shop.name);
            }
        }
    }

    public class AdmOrgVarWork : OrgVarWork
    {
        public AdmOrgVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(WebContext ac)
        {
            string orgid = ac[this];
            const byte proj = Org.ADM;
            if (ac.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Org>(dc.Sql("SELECT ").columnlst(Org.Empty, proj).T(" FROM orgs WHERE id = @1"), p => p.Set(orgid), proj);
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        m.FIELD(o.id, "编号");
                        m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                        m.SELECT(nameof(o.city), o.city, City.All, "城市", refresh: true);
                        m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                        m.NUMBER(nameof(o.x), o.x, "经度", box: 6).NUMBER(nameof(o.x), o.x, "纬度", box: 6);
                        m._FORM();
                    });
                }
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Org>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Execute(dc.Sql("UPDATE orgs")._SET_(Org.Empty, proj ^ Org.ID).T(" WHERE id = @1"), p =>
                    {
                        o.Write(p, proj ^ Org.ID);
                        p.Set(orgid);
                    });
                }
                ac.GivePane(200);
            }
        }

        [Ui("经理"), Tool(ButtonShow)]
        public async Task mgr(WebContext ac)
        {
            string orgid = ac[this];
            string wx_tel_name;
            if (ac.GET)
            {
                string forid = ac.Query[nameof(forid)];
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.FIELDSET_("查询帐号（手机号）");
                    m.SEARCH(nameof(forid), forid, pattern: "[0-9]+", max: 11, min: 11);
                    m._FIELDSET();
                    if (forid != null)
                    {
                        using (var dc = NewDbContext())
                        {
                            if (dc.Query1("SELECT concat(wx, ' ', tel, ' ', name) FROM users WHERE tel = @1", p => p.Set(forid)))
                            {
                                dc.Let(out wx_tel_name);
                                m.FIELDSET_("设置经理");
                                m.RADIO(nameof(wx_tel_name), wx_tel_name, wx_tel_name);
                                m._FIELDSET();
                            }
                        }
                    }
                    m._FORM();
                });
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                wx_tel_name = f[nameof(wx_tel_name)];
                (string wx, string tel, string name) = wx_tel_name.ToTriple();
                using (var dc = NewDbContext())
                {
                    dc.Execute(@"UPDATE orgs SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; 
                        UPDATE users SET opr = " + OPRMGR + ", oprat = @4 WHERE wx = @1;", p => p.Set(wx).Set(tel).Set(name).Set(orgid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("照片"), Tool(ButtonCrop)]
        public new async Task icon(WebContext ac)
        {
            string orgid = ac[this];
            if (ac.GET)
            {
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM orgs WHERE id = @1", p => p.Set(orgid)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else
                            ac.Give(200, new StaticContent(byteas));
                    }
                    else ac.Give(404); // not found           
                }
                return;
            }

            var f = await ac.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = NewDbContext())
            {
                dc.Execute("UPDATE orgs SET icon = @1 WHERE id = @2", p => p.Set(jpeg).Set(orgid));
            }
            ac.Give(200); // ok
        }
    }
}