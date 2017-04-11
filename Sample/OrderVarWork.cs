using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public abstract class OrderVarWork : Work
    {
        protected OrderVarWork(WorkContext ctx) : base(ctx)
        {
        }

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

        [Ui("取消")]
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

        [Ui("已备货")]
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


        [Ui]
        public void exam(ActionContext ac)
        {
        }
    }

    public abstract class MyOrderVarWork : OrderVarWork
    {
        public MyOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    /// <summary>
    /// About a single cart order targeted one shop.
    /// </summary>
    /// <code>
    /// /my//cart/-ordid-/
    /// </code>
    public class MyCartOrderVarWork : MyOrderVarWork
    {
        public MyCartOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }

        [Ui("付款", UiMode.AnchorScript)]
        public async Task prepay(ActionContext ac)
        {
            long ordid = ac[this];
            string buywx = ((User) ac.Principal).wx;

            using (var dc = ac.NewDbContext())
            {
                string prepay_id = null;
                decimal total = 0;
                if (dc.Query1("SELECT prepay_id, total FROM orders WHERE id = @1 AND buywx = @2", p => p.Set(ordid).Set(buywx)))
                {
                    prepay_id = dc.GetString();
                    total = dc.GetDecimal();
                    if (prepay_id == null) // if not yet, call unifiedorder remotely
                    {
                        prepay_id = await WeiXinUtility.PostUnifiedOrderAsync(ordid, total, null, "http://shop.144000.tv/notify");
                        dc.Execute("UPDATE orders SET prepay_id = @1 WHERE id = @2", p => p.Set(prepay_id).Set(ordid));
                    }

                    ac.Give(200, WeiXinUtility.MakePrepayContent(prepay_id));
                }
                else
                {
                    ac.Give(404, "order not found");
                }
            }
        }
    }

    public class MyRealOrderVarWork : MyOrderVarWork
    {
        public MyRealOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public abstract class OprOrderVarWork : OrderVarWork
    {
        protected OprOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class OprUnpaidOrderVarWork : OprOrderVarWork
    {
        public OprUnpaidOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class OprPaidOrderVarWork : OprOrderVarWork
    {
        public OprPaidOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }

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
    }

    public class OprFixedOrderVarWork : OprOrderVarWork
    {
        public OprFixedOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class OprClosedOrderVarWork : OprOrderVarWork
    {
        public OprClosedOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class OprAbortedOrderVarWork : OprOrderVarWork
    {
        public OprAbortedOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public abstract class DvrOrderVarWork : OrderVarWork
    {
        protected DvrOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class DvrReadyOrderVarWork : DvrOrderVarWork
    {
        public DvrReadyOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class DvrShippedOrderVarWork : DvrOrderVarWork
    {
        public DvrShippedOrderVarWork(WorkContext ctx) : base(ctx)
        {
        }
    }
}