using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        public OrderWork(WorkContext wc) : base(wc)
        {
            CreateVar<V>();
        }

        public virtual void @default(ActionContext ac, int page)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = 2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveWorkPage(Parent, 200, dc.ToList<Order>());
                }
                else
                {
                    ac.GiveWorkPage(Parent, 200, (List<Order>)null);
                }
            }
        }

        public async Task notify(ActionContext ac)
        {
            XElem xe = await ac.ReadAsync<XElem>();
            string appid = xe[nameof(appid)];
            string mch_id = xe[nameof(mch_id)];
            string openid = xe[nameof(openid)];
            string nonce_str = xe[nameof(nonce_str)];
            string sign = xe[nameof(sign)];
            string result_code = xe[nameof(result_code)];

            string bank_type = xe[nameof(bank_type)];
            string total_fee = xe[nameof(total_fee)]; // 订单总金额单位分
            string cash_fee = xe[nameof(cash_fee)]; // 支付金额单位分
            string transaction_id = xe[nameof(transaction_id)]; // 微信支付订单号
            string out_trade_no = xe[nameof(out_trade_no)]; // 商户订单号
            string time_end = xe[nameof(time_end)]; // 支付完成时间

        }

        [Ui("核对付款")]
        public async Task check(ActionContext ac)
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

        [Ui]
        public void ask(ActionContext ac)
        {
            // string shopid = wc.Var(null);
        }

    }

    [Ui("购物车")]
    public class MyCartOrderWork : MyOrderWork<MyCartOrderVarWork>
    {
        public MyCartOrderWork(WorkContext wc) : base(wc)
        {
        }

        [Ui]
        public void ask(ActionContext ac)
        {
            // string shopid = wc.Var(null);
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

    public abstract class MgrOrderWork<V> : OrderWork<V> where V : MgrOrderVarWork
    {
        public MgrOrderWork(WorkContext wc) : base(wc)
        {
        }

    }

    public class MgrUnpaidOrderWork : MgrOrderWork<MgrUnpaidOrderVarWork>
    {
        public MgrUnpaidOrderWork(WorkContext wc) : base(wc)
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
    public class MgrPaidOrderWork : MgrOrderWork<MgrPaidOrderVarWork>
    {
        public MgrPaidOrderWork(WorkContext wc) : base(wc)
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

    [Ui("已锁")]
    public class MgrFixedOrderWork : MgrOrderWork<MgrLockedOrderVarWork>
    {
        public MgrFixedOrderWork(WorkContext wc) : base(wc)
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
    public class MgrClosedOrderWork : MgrOrderWork<MgrClosedOrderVarWork>
    {
        public MgrClosedOrderWork(WorkContext wc) : base(wc)
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
    public class MgrAbortedOrderWork : MgrOrderWork<MgrAbortedOrderVarWork>
    {
        public MgrAbortedOrderWork(WorkContext wc) : base(wc)
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