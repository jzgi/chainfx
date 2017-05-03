using System;
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
                const int proj = -1 ^ Order.LATE ^ Order.CUSTWX;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE custwx = @1 AND status = @2 ORDER BY id DESC");
                if (dc.Query(p => p.Set(wx).Set(Order.CREATED)))
                {
                    ac.GiveGridFormPage(200, dc.ToArray<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (Order[]) null);
                }
            }
        }

        [Ui("清空购物车", UiMode.ButtonConfirm)]
        public void empty(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE custwx = @1 AND status = @2", p => p.Set(wx).Set(Order.CREATED));
                ac.GiveRedirect();
            }
        }

        public async Task add(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            string shopid = ac.Query[nameof(shopid)];
            string name = ac.Query[nameof(name)];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    const short proj = -1 ^ Item.QTY ^ Item.ICON;
                    dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1 AND name = @2");
                    if (dc.Query1(p => p.Set(shopid).Set(name)))
                    {
                        var item = dc.ToObject<Item>(proj);
                        short qty = item.min;
                        ac.GiveFormPane(200, h =>
                        {
                            h.Add(name);
                            h.NUMBER(nameof(qty), qty, label: "数量", min: item.min, step: item.step, required: true);
                            h.HIDDEN(nameof(item.shopname), item.shopname);
                            h.HIDDEN(nameof(item.unit), item.unit);
                            h.HIDDEN(nameof(item.price), item.price);
                        });
                    }
                    else ac.Give(404); // not found
                }
            }
            else // process post
            {
                var item = await ac.ReadObjectAsync<Item>(-1 ^ Item.ICON);
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT id, detail, total FROM orders WHERE shopid = @1 AND custwx = @2 AND status = 0", p => p.Set(shopid).Set(wx)))
                    {
                        var order = new Order
                        {
                            id = dc.GetLong(),
                            detail = dc.GetArray<OrderLine>(),
                            total = dc.GetDecimal()
                        };
                        order.AddItem(name, item.qty, item.unit, item.price);
                        order.Sum();
                        dc.Execute("UPDATE orders SET detail = @1, total = @2 WHERE id = @3", p => p.Set(order.detail).Set(order.total).Set(order.id));
                    }
                    else
                    {
                        User prin = (User) ac.Principal;
                        var order = new Order
                        {
                            shopid = shopid,
                            shopname = item.shopname,
                            custname = prin.name,
                            custwx = prin.wx,
                            custtel = prin.tel,
                            custcity = prin.city,
                            custdistr = prin.distr,
                            custaddr = prin.addr,
                            detail = new[]
                            {
                                new OrderLine {item = name, price = item.price, qty = item.qty, unit = item.unit}
                            },
                            created = DateTime.Now
                        };
                        order.Sum();

                        const int proj = -1 ^ Order.ID ^ Order.LATE;

                        dc.Sql("INSERT INTO orders ")._(order, proj)._VALUES_(order, proj);
                        dc.Execute(p => order.WriteData(p, proj));
                    }
                    ac.GivePane(200, null);
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
                if (dc.Query(p => p.Set(wx).Set(Order.RECEIVED).Set(Order.SENT)))
                {
                    ac.GiveGridFormPage(200, dc.ToArray<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (Order[]) null);
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
                    ac.GiveGridFormPage(200, dc.ToArray<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridFormPage(200, (Order[]) null);
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
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(status).Set(page * 20)))
                {
                    ac.GiveGridFormPage(200, dc.ToArray<Order>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (Order[]) null);
                }
            }
        }
    }

    [Ui("在购")]
    public class OprCartOrderWork : OprOrderWork<OprCartOrderVarWork>
    {
        public OprCartOrderWork(WorkContext wc) : base(wc, Order.CREATED)
        {
        }

        [Ui("清除半月", UiMode.ButtonConfirm)]
        public void clear(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE shopid = @1 AND status = @2 AND age(created) > '15 days'", p => p.Set(shopid).Set(Order.CREATED));
                ac.GiveRedirect(null);
            }
        }
    }

    [Ui("到款")]
    public class OprPaidOrderWork : OprOrderWork<OprPaidOrderVarWork>
    {
        public OprPaidOrderWork(WorkContext wc) : base(wc, Order.RECEIVED)
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

    [Ui("在途")]
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

    [Ui("已往")]
    public class OprHistoryOrderWork : OprOrderWork<OprHistoryOrderVarWork>
    {
        public OprHistoryOrderWork(WorkContext wc) : base(wc, Order.DONE)
        {
        }
    }

    public abstract class DvrOrderWork<V> : OrderWork<V> where V : DvrOrderVarWork
    {
        protected readonly short status;

        protected DvrOrderWork(WorkContext wc, short status) : base(wc)
        {
            this.status = status;
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND dvr = #2 AND status = @3 ORDER BY id LIMIT 20 OFFSET @4", p => p.Set(shopid).Set(status).Set(page * 20)))
                {
                    ac.GiveGridFormPage(200, dc.ToArray<Order>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (Order[]) null);
                }
            }
        }
    }


    [Ui("安排在派")]
    public class DvrSentOrderWork : DvrOrderWork<DvrSentOrderVarWork>
    {
        public DvrSentOrderWork(WorkContext wc) : base(wc, Order.SENT)
        {
        }


        [Ui("驳回安排")]
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

    [Ui("派送成功")]
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