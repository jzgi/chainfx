using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /shop/-id-/
    ///
    public class ShopVarFolder : WebFolder, IVar
    {
        readonly WebAction _re_menu_;

        public ShopVarFolder(WebFolderContext dc) : base(dc)
        {
            Create<OrderFolder>("order");

            Create<ItemFolder>("item");

            _re_menu_ = GetAction(nameof(remenu));
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
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty)._("FROM items WHERE @shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Set(shopid)))
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
        /// Get products and submit to basket.
        ///
        /// <code>
        /// GET /010001/
        /// </code>
        ///
        public void @default(WebActionContext ac)
        {
            string shopid = ac[0];
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Set(shopid)))
                {
                    var items = dc.ToArray<Item>();
                    ac.ReplyPage(200, "", main =>
                    {
                        main.FORM(_re_menu_, p =>
                        {

                        });
                    });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        #region -shop-management-

        [ToShop]
        public void remenu(WebActionContext ac)
        {
        }


        public void basket(WebActionContext ac)
        {
        }

        public void invoice(WebActionContext ac)
        {
        }

        #endregion
    }
}