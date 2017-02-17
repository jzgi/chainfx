using Greatbone.Core;
using System.Collections.Generic;

namespace Greatbone.Sample
{
    ///
    /// /shop/-shopid-/
    ///
    public class ShopVarFolder : WebFolder, IVar
    {
        public ShopVarFolder(WebFolderContext dc) : base(dc)
        {
            Create<OrderFolder>("orderi"); // order inbox

            Create<OrderFolder>("ordero"); // order old history

            Create<ItemFolder>("item");

            Create<RepayFolder>("repay");
        }


        public void @default(WebActionContext ac)
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
        public void items(WebActionContext ac)
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

        [Shop]
        public void remenu(WebActionContext ac)
        {
        }


        public void basket(WebActionContext ac)
        {
        }

        public void invoice(WebActionContext ac)
        {
        }
    }
}