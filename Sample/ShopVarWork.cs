using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.SampUtility;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
{
    public abstract class ShopVarWork : Work
    {
        protected ShopVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                }
                else ac.Give(404, @public: true, maxage: 60 * 5); // not found
            }
        }

        public void img(ActionContext ac, int ordinal)
        {
            string shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM shops WHERE id = @1", p => p.Set(shopid)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                }
                else ac.Give(404, @public: true, maxage: 60 * 5); // not found
            }
        }
    }

    public class PubShopVarWork : ShopVarWork, IShopVar
    {
        public PubShopVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<PubItemVarWork, string>(obj => ((Item) obj).name);
        }

        [Ui("进入该网点"), Tool(Anchor)]
        public void @default(ActionContext ac)
        {
            string shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                var sh = dc.Query1<Shop>(dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1"), p => p.Set(shopid));
                ac.Register(sh);

                var items = dc.Query<Item>(dc.Sql("SELECT ").columnlst(Item.Empty, -1).T(", img1 IS NOT NULL AS imgg FROM items WHERE shopid = @1 AND status > 0 ORDER BY status DESC"), p => p.Set(shopid), -1);
                ac.GiveDoc(200, m =>
                {
                    m.TOPBAR_().T("<a class=\"back-arrow\" href=\"../?city=").T(sh.city).T("\">❮</a>&nbsp;");

                    m.A_DROPDOWN_("网点概况");
                    m.BOX_(0x4c);
                    m.P(Shop.Statuses[sh.status], "状态");
                    m.P_("派送").T(sh.delivery);
                    if (sh.areas != null) m.SEP().T("限送").T(sh.areas);
                    m._P();
                    m.P(sh.schedule, "营业");
                    if (sh.off > 0)
                        m.P_("促销").T(sh.min).T("元起送，满").T(sh.notch).T("元减").T(sh.off).T("元")._P();
                    m._BOX();
                    m.QRCODE(NETADDR + ac.Uri, box: 0x15);
                    if (sh.oprtel != null)
                        m.FIELD_(box: 7).A("&#128222;" + sh.oprtel, "tel:" + sh.oprtel + "#mp.weixin.qq.com", false)._FIELD();
                    m._A_DROPDOWN();

                    m._TOPBAR();

                    if (items == null) return;
                    m.BOARDVIEW(items, (h, o) =>
                    {
                        h.CAPTION(o.name);
                        h.ICON((o.name) + "/icon", box: 4);
                        h.BOX_(0x48).P(o.descr, "特色").P(o.content, "主含").P_().EM(o.price, "¥").T("（还剩").T(o.stock).T(o.unit).T("）")._P()._BOX();
                        if (o.imgg)
                        {
                            h.THUMBNAIL(o.name + "/img-1", box: 3).THUMBNAIL(o.name + "/img-2", box: 3).THUMBNAIL(o.name + "/img-3", box: 3).THUMBNAIL(o.name + "/img-4", box: 3);
                        }
                        h.TAIL();
                        // adjust item availability
                        if (sh.status == 0) o.stock = 0;
                    });
                }, true, 60 * 5, sh.name);
            }
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string shopid = ac[this];
            const short proj = Shop.ADM;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var o = dc.Query1<Shop>(dc.Sql("SELECT ").columnlst(Shop.Empty, proj).T(" FROM shops WHERE id = @1"), p => p.Set(shopid), proj);
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
                var o = await ac.ReadObjectAsync<Shop>(proj);
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj ^ Shop.ID).T(" WHERE id = @1"), p =>
                    {
                        o.Write(p, proj ^ Shop.ID);
                        p.Set(shopid);
                    });
                }
                ac.GivePane(200);
            }
        }

        [Ui("经理"), Tool(ButtonShow)]
        public async Task mgr(ActionContext ac)
        {
            string shopid = ac[this];
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
                        using (var dc = ac.NewDbContext())
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
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; 
                        UPDATE users SET opr = " + OPRMGR + ", oprat = @4 WHERE wx = @1;", p => p.Set(wx).Set(tel).Set(name).Set(shopid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("照片"), Tool(ButtonCrop)]
        public new async Task icon(ActionContext ac)
        {
            string shopid = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
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
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2", p => p.Set(jpeg).Set(shopid));
            }
            ac.Give(200); // ok
        }
    }
}