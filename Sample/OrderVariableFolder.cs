using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /order/-id-/
    ///
    public class OrderVariableFolder : WebFolder, IVariable
    {
        public OrderVariableFolder(WebFolderContext dc) : base(dc)
        {
        }

        ///
        /// Get order's detail.
        ///
        public void @default(WebActionContext ac)
        {
            string shopid = ac.Key;
            int id = ac.Key;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(sql.ToString(), p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                    ac.ReplyPage(200, "", main =>
                    {

                    });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        public void cannel(WebActionContext ac)
        {
            string shopid = ac.Key;
            int orderid = ac.Key;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(sql.ToString(), p => p.Set(orderid).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                    ac.ReplyPage(200, "", main =>
                    {

                    });
                }
                else
                    ac.ReplyPage(200, "没有记录", main => { });
            }
        }

        public void pend(WebActionContext ac)
        {
        }

        public void close(WebActionContext ac)
        {
        }
    }
}