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
            short id = ac[this];
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(id)))
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
            short shopid = ac[this];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Shop.Empty)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToObject<Shop>();

                    // items of the shop

                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToArray<Item>();
                    }

                    ac.GivePage(200, h =>
                    {
                        h.T("<div data-sticky-container>");
                        h.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        h.T("<div class=\"top-bar\">");
                        h.T("<div class=\"top-bar-title\">").T(shop.name).T("</div>");
                        h.T("<div class=\"top-bar-left\">").T("<a href=\"tel:").T(shop.oprtel).T("#mp.weixin.qq.com\">电话&nbsp;").T(shop.oprtel).T("</a></div>");
                        h.T("<div class=\"top-bar-right\">");
                        h.T("<a class=\"float-right\" href=\"/my//pre/\"><i class=\"fi-shopping-cart\" style=\"font-size: 1.75rem; line-height: 2rem\"></i></a>");
                        h.T("</div>");
                        h.T("</div>");
                        h.T("</div>");
                        h.T("</div>");

                        // display items
                        if (items == null)
                        {
                            h.T("没有上架商品");
                            return;
                        }

                        h.T("<div class=\"grid-x small-up-1 medium-up-2\">");
                        for (int i = 0; i < items.Length; i++)
                        {
                            h.T("<div class=\"cell card-board\">");
                            var item = items[i];

                            var shopname = shop.name;

                            h.T("<div class=\"grid-x card\">");

                            h.T("<div class=\"small-12 card-cap\"><h3>").T(item.name).T("</h3></div>");

                            h.T("<div class=\"small-4 cell\">");
                            h.T("<img src=\"").T(item.name).T("/icon\" alt=\"\" class=\"thumbnail circle\">");
                            h.T("</div>"); // column

                            h.T("<div class=\"small-8 cell\">");
                            h.T("<h4>");
                            h.T(item.name);
                            if (item.max > 0)
                            {
                                h.T("（").T(item.max).T(item.unit).T("）");
                            }
                            h.T("</h4>");
                            h.T("<div>");
                            h.T(item.descr);
                            h.T("</div>");

                            h.T("<p>");
                            h.T("<strong class=\"money\">&yen;").T(item.price).T("</strong> ");
                            h.T(item.unit);
                            h.T("</p>");

                            h.T("<div class=\"row\">");

                            h.T("<div class=\"small-5 cell\">");

                            h.HIDDEN(nameof(shopid), shopid);
                            h.HIDDEN(nameof(shopname), shopname);
                            h.HIDDEN(nameof(item.name), item.name);
                            h.HIDDEN(nameof(item.unit), item.unit);
                            h.HIDDEN(nameof(item.price), item.price);

                            h.T("<button type=\"button\" class=\"button hollow primary float-right\"  data-toggle=\"dropdown").T(i).T("\">+</button>");
                            h.T("<div class=\"dropdown-pane\" id=\"dropdown").T(i).T("\" data-position=\"top\" data-alignment=\"right\" style=\"box-shadow:0 0 2px #0a0a0a;\" data-dropdown>");
                            h.T("<form>");

                            h.NUMBER(nameof(item.max), item.min, min: item.min, step: item.step);

                            if (item.customs != null)
                            {
                                h.CHECKBOXGROUP(nameof(item.customs), null, item.customs, "定制要求");
                            }

                            h.T("<button type=\"button\" class=\"button primary\" >加入购物车</button>");
                            h.T("</form>");
                            h.T("</div>");

                            h.T("</div>");
                            h.T("</div>"); // row

                            h.T("</div>"); // column

                            h.T("</div>"); // row card
                            h.T("</div>");
                        }
                        h.T("</div>");
                    }, true, 60 * 5);
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

        [Ui("修改", Mode = ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            short id = ac[this];
            const short proj = Shop.INITIAL;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                    if (dc.Query1(p => p.Set(id)))
                    {
                        var o = dc.ToObject<Shop>(proj);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXT(nameof(o.name), o.name, "名称");
                            m.SELECT(nameof(o.city), o.city, ((SampleService) Service).Cities, "城市");
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
                    dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj)._("WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(id);
                    });
                }
                ac.GivePane(200);
            }
        }

        [Ui("经理", Mode = ButtonShow)]
        public async Task mgr(ActionContext ac)
        {
            short shopid = ac[this];
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

        [Ui("图片", Mode = ACrop)]
        public new async Task icon(ActionContext ac)
        {
            short shopid = ac[this];
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