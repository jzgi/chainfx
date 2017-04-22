using System.Collections.Generic;
using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    [User]
    public abstract class ShopVarWork : Work
    {
        protected ShopVarWork(WorkContext wc) : base(wc)
        {
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

    public class PubShopVarWork : ShopVarWork
    {
        public PubShopVarWork(WorkContext wc) : base(wc)
        {
            CreateVar<PubItemVarWork, string>();
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                // query for the shop record
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @1");
                if (dc.Query1(p => p.Set(shopid)))
                {
                    var shop = dc.ToObject<Shop>(proj);

                    // query for item records of the shop
                    List<Item> items = null;
                    dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        items = dc.ToList<Item>(proj);
                    }

                    ac.GivePage(200, m =>
                    {
                        m.Add("<div data-sticky-container>");
                        m.Add("<div class=\"sticky\" style=\"width: 100%\" data-sticky  data-options=\"anchor: page; marginTop: 0; stickyOn: small;\">");
                        m.Add("<div class=\"title-bar\">");
                        m.Add("<div class=\"title-bar-left\">");
                        m.Add("<a href=\"../\" onclick=\"return dialog(this, 2);\">"); m.Add(shop.name); m.Add("</a>");
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
                        for (int i = 0; i < items.Count; i++)
                        {
                            Item item = items[i];
                            m.Add("<form id=\"item");
                            m.Add(i);
                            m.Add("\">");
                            m.Add("<div class=\"row\">");

                            m.Add("<div class=\"small-4 columns\"><a href=\"#\"><span></span><img src=\"");
                            m.Add(item.name);
                            m.Add("/_icon_\" alt=\"\" class=\" thumbnail\"></a></div>");
                            m.Add("<div class=\"small-8 columns\">");
                            m.Add("<p>&yen;");
                            m.Add(item.price);
                            m.Add("</p>");
                            m.Add("<p>");
                            m.Add(item.descr);
                            m.Add("</p>");

                            m.Add("<a class=\"button warning\" href=\"");
                            m.Add(item.name);
                            m.Add("/add\" onclick=\"return dialog(this,2)\">加入购物车</a>");
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
            Create<OprPaidOrderWork>("paid");

            Create<OprPackedOrderWork>("packed");

            Create<OprAbortedOrderWork>("aborted");

            Create<OprAssignedOrderWork>("assigned");

            Create<OprDoneOrderWork>("done");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }

    [Ui("设置")]
    public class DvrShopVarWork : ShopVarWork
    {
        public DvrShopVarWork(WorkContext wc) : base(wc)
        {
            Create<DvrAssignedOrderWork>("assigned");

            Create<DvrDoneOrderWork>("done");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }

    public class MgrShopVarWork : ShopVarWork
    {
        public MgrShopVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class AdmShopVarWork : ShopVarWork
    {
        public AdmShopVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}