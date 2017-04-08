using Greatbone.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        public OrderWork(WorkContext wc) : base(wc)
        {
            CreateVar<V>();
        }

        [Ui("标注完成")]
        public void close(ActionContext ac)
        {
        }

        [Ui("取消")]
        public async Task cancel(ActionContext ac)
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

        [Ui]
        public void clear(ActionContext ac)
        {
            // string shopid = wc.Var(null);

        }


        [Ui]
        public void exam(ActionContext ac)
        {

        }

    }

    public abstract class MyOrderWork<V> : OrderWork<V> where V : MyOrderVarWork
    {
        public MyOrderWork(WorkContext wc) : base(wc)
        {
        }

    }

    [Ui("购物车")]
    public class MyCartOrderWork : MyOrderWork<MyCartOrderVarWork>
    {
        public MyCartOrderWork(WorkContext wc) : base(wc)
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
                    ac.GiveGridFormPage(200, (List<Order>)null);
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
    public class MyRestOrderWork : MyOrderWork<MyCartOrderVarWork>
    {
        public MyRestOrderWork(WorkContext wc) : base(wc)
        {
        }

        [Ui]
        public void ask(ActionContext ac)
        {
            // string shopid = wc.Var(null);
        }

    }

    public abstract class OprOrderWork<V> : OrderWork<V> where V : OprOrderVarWork
    {
        public OprOrderWork(WorkContext wc) : base(wc)
        {
        }

    }

    public class OprUnpaidOrderWork : OprOrderWork<OprUnpaidOrderVarWork>
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
    public class OprPaidOrderWork : OprOrderWork<OprPaidOrderVarWork>
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
                    ac.GiveGridFormPage(200, (List<Order>)null);
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
    public class OprFixedOrderWork : OprOrderWork<OprLockedOrderVarWork>
    {
        public OprFixedOrderWork(WorkContext wc) : base(wc)
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
        public OprClosedOrderWork(WorkContext wc) : base(wc)
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

    [Ui("已撤")]
    public class OprAbortedOrderWork : OprOrderWork<OprAbortedOrderVarWork>
    {
        public OprAbortedOrderWork(WorkContext wc) : base(wc)
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