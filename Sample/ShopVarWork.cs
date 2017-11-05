using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiStyle;

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
                    ArraySegment<byte> byteas;
                    dc.Let(out byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        StaticContent cont = new StaticContent(byteas);
                        ac.Give(200, cont, pub: true, maxage: 60 * 5);
                    }
                }
                else ac.Give(404, pub: true, maxage: 60 * 5); // not found
            }
        }
    }

    public class PubShopVarWork : ShopVarWork
    {
        public PubShopVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<PubItemVarWork, string>(obj => ((Item) obj).name);
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToObject<Shop>();
                    var shopname = shop.name;

                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToArray<Item>();
                    }

                    ac.GivePage(200, main =>
                    {
                        main.TOPBAR_(shop.name).LEFT_().T("<a href=\"tel:").T(shop.oprtel).T("#mp.weixin.qq.com\">电话&nbsp;").T(shop.oprtel).T("</a>")._LEFT()._TOPBAR();

                        // display items
                        if (items == null)
                        {
                            main.T("没有上架商品");
                            return;
                        }

                        main.FORMLIST(items, (h, o) =>
                        {
                            h.HIDDEN(nameof(shopid), shopid);
                            h.HIDDEN(nameof(shopname), shopname);
                            h.HIDDEN(nameof(o.name), o.name);
                            h.HIDDEN(nameof(o.unit), o.unit);
                            h.HIDDEN(nameof(o.price), o.price);

                            h.CAPTION(o.name);
                            h.IMG((o.name) + "/icon", 4).FIELD_(8).P(o.descr).STRONG(o.price, '¥')._FIELD();
                            h.FIELD_().T("<a type=\"button hollow\" class=\"button circle primary float-right\"  data-toggle=\"dropdown").T(o.name).T("\">购买</a>")._FIELD();
                            h.T("<div class=\"dropdown-pane\" id=\"dropdown").T(o.name).T("\" data-position=\"top\" data-alignment=\"right\" style=\"box-shadow:0 0 2px #0a0a0a;\" data-dropdown>");
                            h.T("<form>");

                            h.NUMBER(nameof(o.max), o.min, min: o.min, step: o.step);

                            if (o.customs != null)
                            {
                                h.CHECKBOXGROUP(nameof(o.customs), null, o.customs, "定制要求");
                            }

                            h.T("<button type=\"button\" class=\"button primary\" >加入购物车</button>");
                            h.T("</form>");
                            h.T("</div>");
                        });
                    });
                }
                else
                {
                    ac.Give(404, pub: true, maxage: 60 * 5); // not found
                }
            }
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Style = ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string shopid = ac[this];
            const short proj = Shop.INITIAL;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj).T(" FROM shops WHERE id = @1");
                    if (dc.Query1(p => p.Set(shopid)))
                    {
                        var o = dc.ToObject<Shop>(proj);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXT(nameof(o.name), o.name, "名称");
                            m.SELECT(nameof(o.city), o.city, City.All, "城市");
                            m.TEXT(nameof(o.addr), o.addr, "地址");
                            m.NUMBER(nameof(o.x), o.x, "经度");
                            m.NUMBER(nameof(o.y), o.y, "纬度");
                            m._FORM();
                        });
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Shop>(proj);
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

        [Ui("经理", Style = ButtonShow)]
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
                    m.SEARCH(nameof(forid), forid, min: 11, max: 11, pattern: "[0-9]+");
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
                (string wx, string tel, string name) = wx_tel_name.ToTriple<string, string, string>();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; UPDATE users SET oprat = @4, opr = " + User.OPRMGR + "  WHERE wx = @2;",
                        p => p.Set(wx).Set(tel).Set(name).Set(shopid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("图片", Style = AnchorCrop)]
        public new async Task icon(ActionContext ac)
        {
            string shopid = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
                    {
                        ArraySegment<byte> byteas;
                        dc.Let(out byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else
                        {
                            ac.Give(200, new StaticContent(byteas));
                        }
                    }
                    else ac.Give(404); // not found           
                }
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                ArraySegment<byte> icon = f[nameof(icon)];
                using (var dc = Service.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2", p => p.Set(icon).Set(shopid));
                }
                ac.Give(200); // ok
            }
        }
    }
}