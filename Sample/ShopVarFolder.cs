using Greatbone.Core;
using System.Collections.Generic;

namespace Greatbone.Sample
{
    [Ui("供应点")]
    public class ShopVarFolder : Folder, IVar
    {
        public ShopVarFolder(FolderContext dc) : base(dc)
        {
            AddSub<OrderFolder>("orderi"); // order inbox

            AddSub<OrderFolder>("orderh"); // order history

            AddSub<ItemFolder>("item");

            AddSub<RepayFolder>("repay");
        }


        public void @default(ActionContext ac)
        {
            ac.ReplyFolderPage(200, (List<Item>)null);
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