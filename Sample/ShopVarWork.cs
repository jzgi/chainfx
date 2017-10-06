using System;
using System.Threading.Tasks;
using Greatbone.Core;

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
            short id = ac[this];
            using (var dc = ac.NewDbContext())
            {
                const int proj = Shop.ID | Shop.INITIAL;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(id)))
                {
                    var shop = dc.ToObject<Shop>(proj);

                    // items of the shop
                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, Item.UNMOD)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(id)))
                    {
                        items = dc.ToArray<Item>(Item.UNMOD);
                    }

                    ac.GivePage(200, m =>
                    {
                        m.T("<div data-sticky-container>");
                        m.T("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.T("<div class=\"top-bar\">");
                        m.T("<div class=\"top-bar-title\">").T(shop.name).T("</div>");
                        m.T("<div class=\"top-bar-left\">").T("<i class=\"typcn typcn-phone\"></i>").T(shop.oprtel).T("</div>");
                        m.T("<div class=\"top-bar-right\">");
                        m.T("<a class=\"float-right\" href=\"/my//pre/\"><i class=\"typcn typcn-shopping-cart\" style=\"font-size: 1.5rem\"></i></a>");
                        m.T("</div>");
                        m.T("</div>");
                        m.T("</div>");
                        m.T("</div>");

                        // display items
                        if (items == null)
                        {
                            m.T("没有上架商品");
                            return;
                        }

                        m.T("<div class=\"grid-x grid-padding-x small-up-1 medium-up-2\">");
                        for (int i = 0; i < items.Length; i++)
                        {
                            m.T("<div =\"cell\">");
                            var item = items[i];
                            m.T("<form>");

                            var shopname = shop.name;

                            m.HIDDEN(nameof(id), id);
                            m.HIDDEN(nameof(shopname), shopname);
                            m.HIDDEN(nameof(item.name), item.name);
                            m.HIDDEN(nameof(item.unit), item.unit);
                            m.HIDDEN(nameof(item.price), item.price);

                            m.T("<div class=\"card\">");

                            m.T("<div class=\"small-4 column\">");
                            m.T("<img src=\"").T(item.name).T("/icon\" alt=\"\" class=\"thumbnail circle\">");
                            m.T("</div>"); // column

                            m.T("<div class=\"small-8 column\">");
                            m.T("<h3>");
                            m.T(item.name);
                            if (item.max > 0)
                            {
                                m.T("（").T(item.max).T(item.unit).T("）");
                            }
                            m.T("</h3>");
                            m.T("<div>");
                            m.T(item.descr);
                            m.T("</div>");

                            m.T("<p>");
                            m.T("<strong class=\"money\">&yen;").T(item.price).T("</strong> ");
                            m.T(item.unit);
                            m.T("</p>");

                            m.T("<div class=\"row\">");

                            m.T("<div class=\"small-7 columns\">");
                            m.NUMBER(nameof(item.max), item.min, min: item.min, step: item.step);
                            m.T("</div>");

                            m.T("<div class=\"small-5 columns\">");
                            m.T("<button type=\"button\" class=\"button primary\" onclick=\"var frm=this.form; $.post('/my//pre/add', $(frm).serialize(), function(data){alert('成功加入购物车'); frm.reset();});\">加入购物车</button>");
                            m.T("</div>");

                            m.T("</div>"); // row

                            m.T("</div>"); // column

                            m.T("</div>"); // row card
                            m.T("</form>");
                            m.T("</div>");
                        }
                        m.T("</div>");
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

        [Ui("修改", Mode = UiMode.ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            short id = ac[this];
            const int proj = Shop.INITIAL;
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

        [Ui("经理", Mode = UiMode.ButtonShow)]
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
                    dc.Execute(@"UPDATE shops SET mgrwx = @1, mgrtel = @2, mgrname = @3 WHERE id = @4; UPDATE users SET oprat = @4, opr = " + User.OPR_MGR + "  WHERE wx = @2;",
                        p => p.Set(wx).Set(tel).Set(name).Set(shopid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("图片", Mode = UiMode.AnchorCrop)]
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