using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkContext ctx) : base(ctx)
        {
            CreateVar<V, long>((obj) => ((Order) obj).id);
        }
    }

    public abstract class MyOrderWork<V> : OrderWork<V> where V : MyOrderVarWork
    {
        protected MyOrderWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    [Ui("购物车")]
    public class MyCartOrderWork : MyOrderWork<MyCartOrderVarWork>
    {
        public MyCartOrderWork(WorkContext ctx) : base(ctx)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE buywx = @1 AND status = 0 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(wx).Set(page * 20)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>(-1), -1);
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
        }

        [Ui("地址")]
        public async Task addr(ActionContext ac)
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

    [Ui("订单")]
    public class MyRealOrderWork : MyOrderWork<MyCartOrderVarWork>
    {
        public MyRealOrderWork(WorkContext ctx) : base(ctx)
        {
        }

        [Ui("取消")]
        public async Task abort(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        var order = dc.ToArray<Order>();
                    }
                    else
                    {
                    }
                }
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET ").setstate()._(" WHERE id IN () AND shopid = @1 AND ").statecond();
                    if (dc.Query(p => p.Set(pk).Set(shopid)))
                    {
                        ac.Give(303); // see other
                    }
                    else
                    {
                        ac.Give(303); // see other
                    }
                }
            }
        }
    }

    public abstract class OprOrderWork<V> : OrderWork<V> where V : OprOrderVarWork
    {
        protected OprOrderWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public class OprUnpaidOrderWork : OprOrderWork<OprUnpaidOrderVarWork>
    {
        public OprUnpaidOrderWork(WorkContext ctx) : base(ctx)
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

    [Ui("已付")]
    public class OprPaidOrderWork : OprOrderWork<OprPaidOrderVarWork>
    {
        public OprPaidOrderWork(WorkContext ctx) : base(ctx)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = 0 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Order>) null);
                }
            }
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

    [Ui("已锁")]
    public class OprFixedOrderWork : OprOrderWork<OprFixedOrderVarWork>
    {
        public OprFixedOrderWork(WorkContext ctx) : base(ctx)
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
    public class OprClosedOrderWork : OprOrderWork<OprClosedOrderVarWork>
    {
        public OprClosedOrderWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    [Ui("已撤")]
    public class OprAbortedOrderWork : OprOrderWork<OprAbortedOrderVarWork>
    {
        public OprAbortedOrderWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    public abstract class DvrOrderWork<V> : OrderWork<V> where V : DvrOrderVarWork
    {
        protected DvrOrderWork(WorkContext ctx) : base(ctx)
        {
        }
    }

    [Ui("待派送")]
    public class DvrReadyOrderWork : DvrOrderWork<DvrReadyOrderVarWork>
    {
        public DvrReadyOrderWork(WorkContext ctx) : base(ctx)
        {
        }

        public async Task @default(ActionContext ac)
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

    [Ui("已派送")]
    public class DvrShippedOrderWork : DvrOrderWork<DvrShippedOrderVarWork>
    {
        public DvrShippedOrderWork(WorkContext ctx) : base(ctx)
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