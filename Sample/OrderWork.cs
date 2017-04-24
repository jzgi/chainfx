using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, long>((obj) => ((Order) obj).id);
        }
    }

    public abstract class MyOrderWork<V> : OrderWork<V> where V : MyOrderVarWork
    {
        protected MyOrderWork(WorkContext wc) : base(wc)
        {
        }
    }


    [Ui("购物车")]
    public class MyCartOrderWork : MyOrderWork<MyCartOrderVarWork>
    {
        public MyCartOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ Projection.LATE;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE custwx = @1 AND status = @2");
                if (dc.Query(p => p.Set(wx).Set(Order.CREATED)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
        }

        [Ui("购物车清空")]
        public async Task empty(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }

    [Ui("当前订单")]
    public class MyCurrentOrderWork : MyOrderWork<MyCurrentOrderVarWork>
    {
        public MyCurrentOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE custwx = @1 AND status BETWEEN @2 AND @3");
                if (dc.Query(p => p.Set(wx).Set(Order.PAID).Set(Order.SENT)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
        }
    }

    [Ui("历史订单")]
    public class MyHistoryOrderWork : MyOrderWork<MyHistoryOrderVarWork>
    {
        public MyHistoryOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE custwx = @1 AND status >= @2 ORDER BY id LIMIT 10 OFFSET @4");
                if (dc.Query(p => p.Set(wx).Set(Order.DONE).Set(page * 10)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
        }
    }

    public abstract class OprOrderWork<V> : OrderWork<V> where V : OprOrderVarWork
    {
        protected readonly short status;

        protected OprOrderWork(WorkContext wc, short status) : base(wc)
        {
            this.status = status;
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT @3 OFFSET @4", p => p.Set(shopid).Set(status).Set(ac.Limit).Set(page * ac.Limit)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
        }
    }

    [Ui("已付")]
    public class OprPaidOrderWork : OprOrderWork<OprPaidOrderVarWork>
    {
        public OprPaidOrderWork(WorkContext wc) : base(wc, Order.PAID)
        {
        }

        [Ui("统计")]
        public async Task calc(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        [Ui("备妥")]
        public async Task packed(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }

    [Ui("已备")]
    public class OprPackedOrderWork : OprOrderWork<OprPackedOrderVarWork>
    {
        public OprPackedOrderWork(WorkContext wc) : base(wc, Order.PACKED)
        {
        }

        [Ui("锁定/处理")]
        public async Task @lock(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }

    [Ui("在派")]
    public class OprSentOrderWork : OprOrderWork<OprSentOrderVarWork>
    {
        public OprSentOrderWork(WorkContext wc) : base(wc, Order.SENT)
        {
        }

        [Ui("锁定/处理")]
        public async Task @lock(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }

    [Ui("已完")]
    public class OprDoneOrderWork : OprOrderWork<OprDoneOrderVarWork>
    {
        public OprDoneOrderWork(WorkContext wc) : base(wc, Order.DONE)
        {
        }
    }

    [Ui("已撤")]
    public class OprAbortedOrderWork : OprOrderWork<OprAbortedOrderVarWork>
    {
        public OprAbortedOrderWork(WorkContext wc) : base(wc, Order.ABORTED)
        {
        }
    }

    [Ui("已派")]
    public class DvrSentOrderWork : OrderWork<DvrSentOrderVarWork>
    {
        public DvrSentOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = 0 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>(-1), -1);
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
        }

        [Ui("反回")]
        public async Task unassign(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }

    [Ui("历史")]
    public class DvrDoneOrderWork : OrderWork<DvrDoneOrderVarWork>
    {
        public DvrDoneOrderWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("锁定/处理")]
        public async Task @lock(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id = @1 AND shopid = @2 AND ").statecond();
                if (dc.Query(p => p.Set(pk).Set(shopid)))
                {
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }
    }
}