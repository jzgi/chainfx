using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/
    ///
    public class ShopFolder : WebFolder
    {
        public ShopFolder(WebFolderContext fc) : base(fc)
        {
            MakeVar<ShopVarFolder>();
        }

        ///
        /// Get items grouped by shop
        ///
        /// <code>
        /// GET /items
        /// </code>
        ///
        public void lst(WebActionContext ac)
        {
            string shopid = ac.Key;

            using (var dc = Service.NewDbContext())
            {
                // shops
                DbSql sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM shops WHERE x = @1 AND NOT disabled");
                if (dc.Query(sql, p => p.Set(shopid)))
                {
                    ac.Reply(200, dc.Dump());
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });

                // products

                sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE @shopid = @1 AND NOT disabled");
                if (dc.Query(sql, p => p.Set(shopid)))
                {
                    var items = dc.ToArray<Item>();
                    ac.ReplyPage(200, "", main =>
                    {
                    });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        ///
        /// Get buyer's personal order list
        ///
        public void all(WebActionContext ac)
        {

        }

        ///
        /// Get shop's order list
        ///
        public void list(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }

        ///
        /// find in shop's order list
        ///
        public void clear(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }
    }
}