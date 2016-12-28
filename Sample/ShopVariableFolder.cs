using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The shop variable folder.
    ///
    public class ShopVariableFolder : WebFolder, IVariable
    {
        readonly WebAction _re_menu_;

        public ShopVariableFolder(WebFolderContext dc) : base(dc)
        {
            Make<BasketFolder>("mycart");

            // order functions
            Make<OrderFolder>("myorder");

            _re_menu_ = Action(nameof(remenu));
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
            string shopid = ac.Var;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Shop.Empty)._("FROM items WHERE @shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Put(shopid)))
                {
                    var items = dc.ToDatas<Item>();
                    ac.ReplyHtmlMajor(200, "", main =>
                    {
                    });
                }
                else
                    ac.ReplyHtmlMajor(200, "没有记录", main => { });
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
            string shopid = ac.Var;
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Put(shopid)))
                {
                    var items = dc.ToDatas<Item>();
                    ac.ReplyHtmlMajor(200, "", main =>
                    {
                        main.Form(_re_menu_, p =>
                        {

                        });
                    });
                }
                else
                    ac.ReplyHtmlMajor(200, "没有记录", main => { });
            }
        }

        ///
        /// recreate menu for this shop with WeChat.
        ///
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