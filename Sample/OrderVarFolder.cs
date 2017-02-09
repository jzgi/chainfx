using Greatbone.Core;
using static Greatbone.Sample.WfOrder;

namespace Greatbone.Sample
{
    ///
    ///
    public class OrderVarFolder : WebFolder, IVar
    {
        public OrderVarFolder(WebFolderContext fc) : base(fc)
        {
        }

        #region /user/-userid-/order/-orderid-/

        public void my(WebActionContext ac)
        {

        }

        [State(PAID, FIXED, ASKED)]
        public void ask(WebActionContext ac)
        {
            string userid = ac[0];
            int orderid = ac[this];
            string reason = null;

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("UPDATE orders SET reason = @1, ").setstate()._(" WHERE id = @2 AND userid = @3 AND ").statecond();
                if (dc.Query(p => p.Set(reason).Set(orderid).Set(userid)))
                {
                    var order = dc.ToArray<WfOrder>();
                    ac.ReplyPage(200, main =>
                    {

                    });
                }
                else
                {
                    ac.ReplyPage(200, main => { });
                }
            }
        }

        #endregion



        #region /shop/-id-/order/-id-/

        [Shop]
        public void @default(WebActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(WfOrder.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<WfOrder>();
                    ac.ReplyPage(200, main =>
                    {

                    });
                }
                else
                {
                    ac.ReplyPage(200, main => { });
                }
            }
        }

        [Ui(Label = "取消")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public void cannel(WebActionContext ac)
        {
            string shopid = ac[0];
            int orderid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(WfOrder.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(orderid).Set(shopid)))
                {
                    var order = dc.ToArray<WfOrder>();
                    ac.ReplyPage(200, main =>
                    {

                    });
                }
                else
                {
                    ac.ReplyPage(200, main => { });
                }
            }
        }

        [Ui(Label = "已备货")]
        [State(ASKED, FIXED | CANCELLED, CANCELLED)]
        public void fix(WebActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToArray<WfOrder>();
                    ac.ReplyPage(200, main => { });
                }
                else
                {
                    ac.ReplyPage(200, main => { });
                }
            }
        }

        public void close(WebActionContext ac)
        {
        }

        #endregion

        #region /order/-id-/

        [Admin, Shop]
        [Ui]
        public void exam(WebActionContext ac)
        {

        }

        #endregion

    }
}