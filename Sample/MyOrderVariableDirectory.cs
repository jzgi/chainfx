using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /-shopid-/myorder/-orderid-/
    ///
    public class MyOrderVariableDirectory : WebDirectory, IVariable
    {
        public MyOrderVariableDirectory(WebMake mk) : base(mk)
        {
        }

        ///
        /// Get order's detail.
        ///
        public void @default(WebExchange wc)
        {
            string shopid = wc.Major;
            int id = wc.Minor;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(sql.ToString(), p => p.Put(id).Put(shopid)))
                {
                    var order = dc.ToData<Order>();
                    wc.SendHtmlMajor(200, "", main =>
                    {

                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
            }
        }

        public void cannel(WebExchange wc)
        {
            string shopid = wc.Major;
            int orderid = wc.Minor;

            using (var dc = Service.NewDbContext())
            {
                DbSql sql = new DbSql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(sql.ToString(), p => p.Put(orderid).Put(shopid)))
                {
                    var order = dc.ToData<Order>();
                    wc.SendHtmlMajor(200, "", main =>
                    {

                    });
                }
                else
                    wc.SendHtmlMajor(200, "没有记录", main => { });
            }
        }

        public void pend(WebExchange wc)
        {
        }

        public void close(WebExchange wc)
        {
        }
    }
}