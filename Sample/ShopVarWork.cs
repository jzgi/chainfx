using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;

namespace Greatbone.Sample
{
    public abstract class ShopVarWork : Work
    {
        protected ShopVarWork(WorkContext wc) : base(wc)
        {
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[this];
            using (var dc = Service.NewDbContext())
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
            using (var dc = Service.NewDbContext())
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

    public class PubShopVarWork : ShopVarWork
    {
        public PubShopVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<PubItemVarWork, string>(obj => ((Item) obj).name);
        }

        [Ui("进入店铺"), Style(Anchor)]
        public void @default(ActionContext ac)
        {
            string shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                dc.Query1(p => p.Set(shopid));
                var shop = dc.ToObject<Shop>();

                dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1 AND status > 0 ORDER BY status DESC");
                dc.Query(p => p.Set(shopid));
                var items = dc.ToArray<Item>();

                ac.GiveDoc(200, m =>
                {
                    m.TOPBAR_(shop.name);
                    if (shop.oprtel != null)
                    {
                        m.T("&nbsp;<a class=\"button hollow\"href=\"tel:").T(shop.oprtel).T("#mp.weixin.qq.com\">&#128222;").T(shop.oprtel).T("</a>");
                    }
                    m._TOPBAR();

                    if (items == null) return;

                    m.GRIDVIEW(items, (h, o) =>
                    {
                        h.CAPTION(o.name);
                        h.ICON((o.name) + "/icon", box: 4);
                        h.BOX_(8).P(o.price, symbol: '¥').P(o.descr, "特色").P(o.mains, "主料")._BOX();

                        // adjust item availability
                        if (shop.status == 0) o.max = 0;
                    });
                });
            }
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改"), Style(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string shopid = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                    if (dc.Query1(p => p.Set(shopid)))
                    {
                        var o = dc.ToObject<Shop>();
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.FIELD(o.id, "编号");
                            m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                            m.SELECT(nameof(o.city), o.city, City.All, "城市", refresh: true);
                            m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                            m.TEXT(nameof(o.schedule), o.schedule, "营业");
                            m.SELECT(nameof(o.marks), o.marks, Mark.All, "特色");
                            m.SELECT(nameof(o.areas), o.areas, City.FindCity(o.city)?.Areas, "限送");
                            m._FORM();
                        });
                    }
                    else ac.Give(500); // internal server error
                }
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>();
                const short proj = -1 ^ Shop.ID;
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj).T(" WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(shopid);
                    });
                }
                ac.GivePane(200);
            }
        }

        [Ui("经理"), Style(ButtonShow)]
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
                    m.BUTTON("查询", false);
                    m._FIELDSET();
                    if (forid != null)
                    {
                        using (var dc = ac.NewDbContext())
                        {
                            if (dc.Query1("SELECT wx, tel, name FROM users WHERE tel = @1", p => p.Set(forid)))
                            {
                                dc.Let(out string wx).Let(out string tel).Let(out string name);
                                m.FIELDSET_("设置经理");
                                m.RADIO(nameof(wx_tel_name), wx, tel, name, false, null, tel, name);
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
                (string wx, string tel, string name) = wx_tel_name.To3Strings('~');
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; UPDATE users SET oprat = @4, opr = " + User.OPRMGR + "  WHERE wx = @2;",
                        p => p.Set(wx).Set(tel).Set(name).Set(shopid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("形象照"), Style(ButtonCrop)]
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
                        {
                            ac.Give(200, new StaticContent(byteas));
                        }
                    }
                    else ac.Give(404); // not found           
                }
                return;
            }

            var f = await ac.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2", p => p.Set(jpeg).Set(shopid));
            }
            ac.Give(200); // ok
        }
    }
}