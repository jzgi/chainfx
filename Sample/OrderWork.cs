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


    [Ui("购物车", "购物车")]
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
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null);
                }
            }
        }

        [Ui("清空购物车", Mode = UiMode.ButtonConfirm)]
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

            var f = await ac.ReadAsync<Form>();
            string shopid = f[nameof(shopid)];
            string shopname = f[nameof(shopname)];
            string name = f[nameof(name)];
            string unit = f[nameof(unit)];
            decimal price = f[nameof(price)];
            short qty = f[nameof(qty)];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT id, detail, total FROM orders WHERE shopid = @1 AND custwx = @2 AND status = 0", p => p.Set(shopid).Set(wx)))
                {
                    var o = new Order
                    {
                        id = dc.GetLong(),
                        detail = dc.GetDatas<OrderLine>(),
                        total = dc.GetDecimal()
                    };
                    o.AddItem(name, qty, unit, price);
                    o.Sum();
                    dc.Execute("UPDATE orders SET detail = @1, total = @2 WHERE id = @3", p => p.Set(o.detail).Set(o.total).Set(o.id));
                }
                else
                {
                    User prin = (User) ac.Principal;
                    var o = new Order
                    {
                        shopid = shopid,
                        shopname = shopname,
                        custname = prin.nickname,
                        custwx = prin.wx,
                        custtel = prin.tel,
                        custcity = prin.city,
                        custdistr = prin.distr,
                        custaddr = prin.addr,
                        detail = new[]
                        {
                            new OrderLine {name = name, price = price, qty = qty, unit = unit}
                        },
                        created = DateTime.Now
                    };
                    o.Sum();

                    const int proj = -1 ^ Order.ID ^ Order.LATE;

                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.WriteData(p, proj));
                }
                ac.Give(200);
            }
        }
    }

    [Ui("当前订单")]
    public class MyPresentOrderWork : MyOrderWork<MyPresentOrderVarWork>
    {
        public MyPresentOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE custwx = @1 AND status BETWEEN @2 AND @3");
                if (dc.Query(p => p.Set(wx).Set(Order.ACCEPTED).Set(Order.SENT)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null);
                }
            }
        }
    }

    [Ui("过去订单")]
    public class MyPastOrderWork : MyOrderWork<MyPastOrderVarWork>
    {
        public MyPastOrderWork(WorkContext wc) : base(wc)
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
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null);
                }
            }
        }
    }

    public abstract class OprOrderWork<V> : OrderWork<V> where V : OprOrderVarWork
    {
        protected short status;

        protected short status2;

        protected short proj;

        protected OprOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                bool found = (status2 == 0) ? dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(status).Set(page * 20)) : dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status BETWEEN @2 AND @3 ORDER BY id LIMIT 20 OFFSET @4", p => p.Set(shopid).Set(status).Set(status2).Set(page * 20));
                if (found)
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null);
                }
            }
        }
    }

    [Ui("购物车")]
    [User(User.ASSISTANT)]
    public class OprCartOrderWork : OprOrderWork<OprCartOrderVarWork>
    {
        public OprCartOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.CREATED;
            proj = -1 ^ Order.LATE ^ Order.CUSTWX;
        }

        [Ui("清理旧单", Mode = UiMode.ButtonConfirm)]
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

    [Ui("已付")]
    [User(User.ASSISTANT)]
    public class OprPaidOrderWork : OprOrderWork<OprAcceptedOrderVarWork>
    {
        public OprPaidOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.ACCEPTED;
            proj = -1 ^ Order.LATE ^ Order.CUSTWX;
        }

        [Ui("统计", Mode = UiMode.AnchorDialog)]
        public async Task calc(ActionContext ac)
        {
            string shopid = ac[-1];
            long[] pk = ac.Query[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
            }
        }

        [Ui("发货")]
        public void send(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            ac.GiveRedirect();
        }
    }

    [Ui("在派")]
    [User(User.DELIVERER)]
    public class OprSentOrderWork : OprOrderWork<OprSentOrderVarWork>
    {
        public OprSentOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.SENT;
            proj = -1 ^ Order.LATE ^ Order.CUSTWX;
        }

        [Ui("锁定/处理")]
        public async Task @lock(ActionContext ac)
        {
            string shopid = ac[0];
            Form frm = await ac.ReadAsync<Form>();
            int[] pk = frm[nameof(pk)];

            using (var dc = ac.NewDbContext())
            {
            }
        }
    }

    [Ui("过去")]
    [User(User.DELIVERER)]
    public class OprPastOrderWork : OprOrderWork<OprHistoryOrderVarWork>
    {
        public OprPastOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.DONE;
            status2 = Order.ABORTED;
            proj = -1 ^ Order.LATE ^ Order.CUSTWX;
        }
    }


    [Ui("代派", "代派别家的订单")]
    [User(User.DELIVERER)]
    public class OprAlienOrderWork : OrderWork<OprAlienOrderVarWork>
    {
        public OprAlienOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>());
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null);
                }
            }
        }
    }
}