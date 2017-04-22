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


    [Ui("购物车")]
    public class MyCartOrderWork : OrderWork<MyCartOrderVarWork>
    {
        public MyCartOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ Projection.LATE;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE buywx = @1 AND status = 0");
                if (dc.Query(p => p.Set(wx)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Order>(proj), proj);
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
    public class MyRealOrderWork : OrderWork<MyCartOrderVarWork>
    {
        public MyRealOrderWork(WorkContext wc) : base(wc)
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

    public class OprUnpaidOrderWork : OrderWork<OprUnpaidOrderVarWork>
    {
        public OprUnpaidOrderWork(WorkContext wc) : base(wc)
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
    public class OprPaidOrderWork : OrderWork<OprPaidOrderVarWork>
    {
        public OprPaidOrderWork(WorkContext wc) : base(wc)
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

    [Ui("已备")]
    public class OprPackedOrderWork : OrderWork<OprPackedOrderVarWork>
    {
        public OprPackedOrderWork(WorkContext wc) : base(wc)
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
    public class OprAssignedOrderWork : OrderWork<OprAssignedOrderVarWork>
    {
        public OprAssignedOrderWork(WorkContext wc) : base(wc)
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
    public class OprDoneOrderWork : OrderWork<OprDoneOrderVarWork>
    {
        public OprDoneOrderWork(WorkContext wc) : base(wc)
        {
        }
    }

    [Ui("已撤")]
    public class OprAbortedOrderWork : OrderWork<OprAbortedOrderVarWork>
    {
        public OprAbortedOrderWork(WorkContext wc) : base(wc)
        {
        }
    }

    [Ui("已派")]
    public class DvrAssignedOrderWork : OrderWork<DvrAssignedOrderVarWork>
    {
        public DvrAssignedOrderWork(WorkContext wc) : base(wc)
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