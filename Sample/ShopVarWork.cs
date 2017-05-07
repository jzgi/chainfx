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
            string shopid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(shopid)))
                {
                    var byteas = dc.GetByteAs();
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        StaticContent cont = new StaticContent(byteas);
                        ac.Give(200, cont);
                    }
                }
                else ac.Give(404); // not found           
            }
        }
    }

    public class PubShopVarWork : ShopVarWork
    {
        public PubShopVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<ItemVarWork, string>(obj => ((Item) obj).name);
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                // query for the shop record
                const int proj = -1 ^ Shop.ICON;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToObject<Shop>(proj);

                    // query for item records of the shop
                    const short projitem = -1 ^ Item.ICON ^ Item.QTY;
                    Item[] items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, projitem)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToArray<Item>(projitem);
                    }

                    ac.GivePage(200, m =>
                    {
                        m.Add("<div data-sticky-container>");
                        m.Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.Add("<div class=\"title-bar\">");
                        m.Add("<div class=\"title-bar-left\">");
                        m.Add("<span style=\"font-size: 1.25rem;\">");
                        m.Add(shop.name);
                        m.Add("</span>");
                        m.Add("</div>");
                        m.Add("<div class=\"title-bar-right\">");
                        m.Add("<a class=\"float-right\" href=\"/my//cart/\"><span class=\"fa-stack fa-lg\"><i class=\"fa fa-circle fa-stack-2x\"></i><i class=\"fa fa-shopping-cart fa-stack-1x fa-inverse\"></i></span></a>");
                        m.Add("</div>");
                        m.Add("</div>");
                        m.Add("</div>");
                        m.Add("</div>");

                        m.Add("<div>");
                        m.Add("<p>");
                        m.Add(shop.city);
                        m.Add(shop.addr);
                        m.Add("</p>");
                        m.Add("<p>");
                        m.Add(shop.descr);
                        m.Add("</p>");
                        m.Add("</div>");

                        // display items

                        if (items == null)
                        {
                            m.Add("没有上架商品");
                            return;
                        }
                        for (int i = 0; i < items.Length; i++)
                        {
                            Item item = items[i];
                            m.Add("<form id=\"item");
                            m.Add(i);
                            m.Add("\">");
                            m.Add("<div class=\"row\">");

                            m.Add("<div class=\"small-4 columns\"><a href=\"#\"><span></span><img src=\"");
                            m.Add(item.name);
                            m.Add("/icon\" alt=\"\" class=\" thumbnail\"></a></div>");
                            m.Add("<div class=\"small-8 columns\">");
                            m.Add("<p>&yen;");
                            m.Add(item.price);
                            m.Add("</p>");
                            m.Add("<p>");
                            m.Add(item.descr);
                            m.Add("</p>");

                            m.Add("<a class=\"button warning\" href=\"/my//cart/add?shopid=");
                            m.Add(shopid);
                            m.Add("&name=");
                            m.Add(item.name);
                            m.Add("\" onclick=\"return dialog(this,2)\">加入购物车</a>");
                            m.Add("</div>");

                            m.Add("</div>");
                            m.Add("</form>");
                        }
                    });
                }
                else
                {
                    ac.Give(404); // not found
                }
            }
        }
    }

    [Ui("设置")]
    public class OprShopVarWork : ShopVarWork
    {
        public OprShopVarWork(WorkContext wc) : base(wc)
        {
            Create<OprCartOrderWork>("cart");

            Create<OprAcceptedOrderWork>("acpt");

            Create<OprSentOrderWork>("sent");

            Create<OprHistoryOrderWork>("history");

            Create<OprAlienOrderWork>("alien");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }

        [Ui("基本资料", UiMode.AnchorDialog)]
        public async Task edit(ActionContext ac)
        {
            const int proj = -1 ^ Shop.ICON ^ Shop.ID ^ Shop.ADM;
            string id = ac[this];
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
                            m.TEXT(nameof(o.name), o.name, label: "商家名称", max: 10, @readonly: true);
                            m.TEXT(nameof(o.descr), o.descr, label: "商家描述", max: 20);
                            m.TEXT(nameof(o.tel), o.tel, label: "电话", max: 11);
                            m.TEXT(nameof(o.city), o.city, label: "城市", @readonly: true);
                            m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), label: "区域");
                            m.TEXT(nameof(o.addr), o.addr, label: "地址");
                            m.SELECT(nameof(o.status), o.status, Shop.STATUS, label: "状态");
                            m._FORM();
                        });
                    }
                    else
                    {
                        ac.Give(404); // not found
                    }
                }
            }
            else // post
            {
                var shop = await ac.ReadObjectAsync<Shop>();
                shop.id = ac[this];
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj)._("WHERE id = @1");
                    dc.Execute(p =>
                    {
                        shop.WriteData(p, proj);
                        p.Set(id);
                    });
                }
                ac.GivePane(200, null);
            }
        }

        [Ui("图片", UiMode.AnchorCrop, Circle = true)]
        public new async Task icon(ActionContext ac)
        {
            string id = ac[this];
            string city = ac[typeof(CityVarWork)];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1", p => p.Set(id)))
                    {
                        var byteas = dc.GetByteAs();
                        if (byteas.Count == 0) ac.Give(204); // no content
                        else
                        {
                            StaticContent cont = new StaticContent(byteas);
                            ac.Give(200, cont);
                        }
                    }
                    else ac.Give(404); // not found
                }
            }
            else // post
            {
                var frm = await ac.ReadAsync<Form>();
                ArraySegment<byte> icon = frm[nameof(icon)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2", p => p.Set(icon).Set(id));
                    ac.Give(200); // ok
                }
            }
        }

        [Ui("人员", UiMode.AnchorDialog)]
        [User(User.ASSISTANT)]
        public void access(ActionContext ac)
        {
        }
    }

    public class SupShopVarWork : ShopVarWork
    {
        public SupShopVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", UiMode.AnchorDialog)]
        public async Task edit(ActionContext ac)
        {
            if (ac.GET)
            {
                string id = ac[this];
                string city = ac[typeof(CityVarWork)];
                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ Shop.ICON;
                    dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1 AND city = @2");
                    if (dc.Query1(p => p.Set(id).Set(city)))
                    {
                        var o = dc.ToObject<Shop>(proj);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.TEXT(nameof(o.name), o.name);
                            m.TEXT(nameof(o.city), o.city, label: "城市", @readonly: true);
                            m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), label: "区域");
                            m.TEXT(nameof(o.lic), o.lic, label: "工商登记");
                            m.TEXT(nameof(o.mgrid), o.mgrid, label: "经理登录号");
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
                var o = await ac.ReadObjectAsync<Shop>();
                o.id = ac[this];
                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ Shop.ICON;
                    dc.Sql("UPDATE shops")._SET_(Shop.Empty, proj)._("WHERE id = @1");
                    dc.Execute(p =>
                    {
                        o.WriteData(p, proj);
                        p.Set(o.id);
                    });
                }
                ac.GiveRedirect();
            }
        }

        [Ui("图片", UiMode.AnchorCrop, Circle = true)]
        public new async Task icon(ActionContext ac)
        {
            string id = ac[this];
            string city = ac[typeof(CityVarWork)];
            if (ac.GET)
            {
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM shops WHERE id = @1 AND name = @2", p => p.Set(id).Set(city)))
                    {
                        var byteas = dc.GetByteAs();
                        if (byteas.Count == 0) ac.Give(204); // no content
                        else
                        {
                            StaticContent cont = new StaticContent(byteas);
                            ac.Give(200, cont);
                        }
                    }
                    else ac.Give(404); // not found
                }
            }
            else // post
            {
                var frm = await ac.ReadAsync<Form>();
                ArraySegment<byte> icon = frm[nameof(icon)];
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Execute("UPDATE shops SET icon = @1 WHERE id = @2 AND city = @3", p => p.Set(icon).Set(id).Set(city)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}