using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// The shop multiplexer directory.
    ///
    public class ShopMuxDir : WebDir, IMux
    {
        readonly WebAction _add;

        public ShopMuxDir(WebDirContext ctx) : base(ctx)
        {
            // customer personal
            Add<MyDir>("my");

            // order functions
            Add<OrderDir>("order");

            _add = GetAction(nameof(register));
        }

        ///
        /// Get products and submit to basket.
        ///
        /// <code>
        /// GET /330001/
        /// </code>
        ///
        public void @default(WebContext wc)
        {
            string shopid = wc.Var;
            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1 AND NOT disabled");
                if (dc.Query(sql.ToString(), p => p.Put(shopid)))
                {
                    var items = dc.ToDatas<Item>();
                    wc.SendHtmlMajor(200, "", main =>
                    {
                        main.form(_add, p =>
                        {

                        });
                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
            }
        }

        ///
        /// recreate menu for this shop with WeChat.
        ///
        public void remenu(WebContext wc)
        {
        }


        public void basket(WebContext wc)
        {
        }

        public void invoice(WebContext wc)
        {
        }
    }
}