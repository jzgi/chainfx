using Greatbone.Core;
using static Greatbone.Sample.Order;

namespace Greatbone.Sample
{
    ///
    ///
    public abstract class OrderVarFolder : Folder, IVar
    {
        public OrderVarFolder(FolderContext fc) : base(fc)
        {
        }

        #region /user/-userid-/order/-orderid-/

        public void my(ActionContext ac)
        {

        }

        [State(PAID, LOCKED, REASONED)]
        public void ask(ActionContext ac)
        {
            string userid = ac[0];
            int orderid = ac[this];
            string reason = null;

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("UPDATE orders SET reason = @1, ").setstate()._(" WHERE id = @2 AND userid = @3 AND ").statecond();
                if (dc.Query(p => p.Set(reason).Set(orderid).Set(userid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        #endregion



        #region /shop/-id-/order/-id-/

        [User]
        public void @default(ActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        [Ui(Label = "取消")]
        [State(REASONED, LOCKED | CANCELLED, CANCELLED)]
        public void cannel(ActionContext ac)
        {
            string shopid = ac[0];
            int orderid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(orderid).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        [Ui(Label = "已备货")]
        [State(REASONED, LOCKED | CANCELLED, CANCELLED)]
        public void fix(ActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        public void close(ActionContext ac)
        {
        }

        #endregion

        #region /order/-id-/

        [User]
        [Ui]
        public void exam(ActionContext ac)
        {

        }

        #endregion

    }

    public class UserOrderVarFolder : OrderVarFolder
    {
        public UserOrderVarFolder(FolderContext fc) : base(fc)
        {
        }
    }

    public class ShopOrderVarFolder : OrderVarFolder
    {
        public ShopOrderVarFolder(FolderContext fc) : base(fc)
        {
        }
    }

}