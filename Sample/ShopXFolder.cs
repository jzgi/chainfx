using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The shop variable folder.
    ///
    public class ShopXFolder : WebFolder, IXable
    {
        readonly WebAction _re_menu_;

        public ShopXFolder(WebFolderContext dc) : base(dc)
        {
            Make<MyCartFolder>("mycart");

            // order functions
            Make<MyOrderFolder>("myorder");

            _re_menu_ = Action(nameof(remenu));
        }

        ///
        /// Get products and submit to basket.
        ///
        /// <code>
        /// GET /330001/
        /// </code>
        ///
        public void @default(WebActionContext ac)
        {
            string shopid = ac.X;
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Put(shopid)))
                {
                    var items = dc.ToDats<Item>();
                    ac.SendHtmlMajor(200, "", main =>
                    {
                        main.Form(_re_menu_, p =>
                        {

                        });
                    });
                }
                else
                    ac.SendHtmlMajor(200, "没有记录", main => { });
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