using Greatbone.Core;

namespace Ministry.Dietary
{
    ///
    /// The shop multiplex directory.
    ///
    public class ShopVariableDir : WebDir, IVariable
    {
        readonly WebAction _add;

        public ShopVariableDir(WebDirContext ctx) : base(ctx)
        {
            // customer personal
            AddChild<MyDir>("my");

            // order functions
            AddChild<OrderDir>("order");

            _add = GetAction(nameof(add));
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
                    wc.SendMajorLayout(200, "", main =>
                    {
                        main.form(_add, p =>
                        {

                        });
                    });
                }
                else
                    wc.SendMajorLayout(200, "没有记录", main => { });
            }
        }

        public void add(WebContext wc)
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