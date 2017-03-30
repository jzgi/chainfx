using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Sample.Order;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        public OrderWork(WorkContext wc) : base(wc)
        {
            CreateVar<V>();
        }

        // [Shop]
        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[typeof(ShopVarWork)];
            short status = Minor;
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(status).Set(page * 20)))
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
        [State(REASONED, LOCKED | CANCELLED, CANCELLED)]
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

        [Ui("设为在处理")]
        [State(REASONED, LOCKED | CANCELLED, CANCELLED)]
        public async Task fix(ActionContext ac)
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
        [State(REASONED, LOCKED | CANCELLED, CANCELLED)]
        public void close(ActionContext ac)
        {
        }

        [Ui("取消")]
        [State(REASONED, LOCKED | CANCELLED, CANCELLED)]
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

        [User]
        [Ui]
        public void clear(ActionContext ac)
        {
            // string shopid = wc.Var(null);

        }


        [User]
        [Ui]
        public void exam(ActionContext ac)
        {

        }

    }

    public class UserOrderWork : OrderWork<UserOrderVarWork>
    {
        public UserOrderWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class ShopOrderWork : OrderWork<ShopOrderVarWork>
    {
        public ShopOrderWork(WorkContext wc) : base(wc)
        {
        }
    }
}