using Greatbone.Core;
using System.Collections.Generic;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    [User]
    [Ui("供应点")]
    public class ShopVarFolder : Folder, IVar
    {
        public ShopVarFolder(FolderContext dc) : base(dc)
        {
            Create<ShopOrderFolder>("order-0", new UiAttribute("当前订单"));

            Create<ShopOrderFolder>("order-2", new UiAttribute("已完成订单"));

            Create<ShopOrderFolder>("order-7", new UiAttribute("已取消订单"));

            Create<ItemFolder>("item", new UiAttribute("货架"));

            Create<ShopRepayFolder>("repay", new UiAttribute("平台结款"));
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                // shop info
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM shops WHERE id = @!");


                // shop items 
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM items ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query(p => p.Set(1)))
                {
                    ac.GiveFolderPage(Parent, 200, dc.ToList<Shop>(proj), proj);
                }
                else
                {
                    ac.GiveFolderPage(Parent, 200, (List<Shop>)null);
                }
            }
        }


        public void _(ActionContext ac)
        {
            ac.GiveFolderPage(this, 200, (List<Item>)null);
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


        //
        // management
        //

        [User]
        public void remenu(ActionContext ac)
        {
        }


        public void basket(ActionContext ac)
        {
        }

        public void invoice(ActionContext ac)
        {
        }
    }
}