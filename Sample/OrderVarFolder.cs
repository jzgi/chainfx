using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public class OrderVarFolder : WebFolder, IVar
    {
        public OrderVarFolder(WebFolderContext fc) : base(fc)
        {
        }

        #region /user/-id-/order/-id-/

        public void my(WebActionContext ac)
        {

        }

        #endregion

        #region /shop/-id-/order/-id-/

        [ToShop]
        public void @default(WebActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
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

        [State()]
        public void pend(WebActionContext ac)
        {
        }

        public void cannel(WebActionContext ac)
        {
            string shopid = ac[0];
            int orderid = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(orderid).Set(shopid)))
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

        public void close(WebActionContext ac)
        {
        }

        #endregion

        #region /order/-id-/

        [ToAdmin, ToShop]
        [Ui]
        public void exam(WebActionContext ac)
        {

        }

        #endregion

    }
}