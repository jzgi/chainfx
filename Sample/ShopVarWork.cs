using Greatbone.Core;
using System.Collections.Generic;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    [User]
    public abstract class ShopVarWork : Work
    {
        public ShopVarWork(WorkContext wc) : base(wc)
        {
        }


        ///
        /// Get shop items
        ///
        /// <code>
        /// GET /-shopid-/items
        /// </code>
        ///
        public void items(ActionContext ac)
        {
            string shopid = ac[0];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Shop.Empty)._("FROM items WHERE @shopid = @1 AND NOT disabled");
                if (dc.Query(p => p.Set(shopid)))
                {
                    var items = dc.ToArray<Item>();
                }
                else
                {
                }
            }
        }

        public void _icon_(ActionContext ac)
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

    /// <summary>
    /// /pub/-shopid-/
    /// </summary>
    public class PubShopVarWork : ShopVarWork
    {
        public PubShopVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<PubItemVarWork>();
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                // shop info
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToObject<Shop>(proj);
                    // shop items 
                    List<Item> items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToList<Item>(proj);
                    }
                    ac.GivePage(200, m =>
                    {
                        m.Add("<div class=\"callout clearfix primary\">");
                        m.Add("<a class=\"float-left\" href=\"../\">"); m.Add(shop.name); m.Add("（切换）</a>");
                        m.Add("<a class=\"float-right\" href=\"/my//cart/\">购物车/付款</a>");
                        m.Add("</div>");

                        m.Add("<div>");
                        m.Add("<p>"); m.Add(shop.city); m.Add(shop.addr); m.Add("</p>");
                        m.Add("<p>"); m.Add(shop.descr); m.Add("</p>");
                        m.Add("</div>");

                        // display items

                        for (int i = 0; i < items.Count; i++)
                        {
                            Item item = items[i];
                            m.Add("<form id=\"item"); m.Add(i); m.Add("\">");
                            m.Add("<div class=\"row\">");

                            m.Add("<div class=\"small-3 columns\"><a href=\"#\"><span></span><img src=\""); m.Add(item.name); m.Add("/_icon_\" alt=\"\" class=\" thumbnail\"></a></div>");
                            m.Add("<div class=\"small-9 columns\">");
                            m.Add("<p>&yen;"); m.Add(item.price); m.Add("</p>");
                            m.Add("<p>"); m.Add(item.descr); m.Add("</p>");

                            m.Add("<button class=\"button warning\" formaction=\""); m.Add(item.name); m.Add("/add\" onclick=\"return dialog(this,2)\">加入购物车</button>");
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

    public class OprShopVarWork : ShopVarWork
    {
        public OprShopVarWork(WorkContext wc) : base(wc)
        {
            Create<OprPaidOrderWork>("paid");

            Create<OprFixedOrderWork>("fixed");

            Create<OprClosedOrderWork>("closed");

            Create<OprAbortedOrderWork>("aborted");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}