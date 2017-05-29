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
                const int proj = -1 ^ Order.LATE ^ Order.WX;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE wx = @1 AND status = @2 ORDER BY id DESC");
                if (dc.Query(p => p.Set(wx).Set(Order.CREATED)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj, @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, @public: false, maxage: 3);
                }
            }
        }

        [Ui("清空/删除", "清空购物车或者删除选中的项", Mode = UiMode.ButtonConfirm)]
        public async Task remove(ActionContext ac)
        {
            string wx = ac[typeof(UserVarWork)];
            var f = await ac.ReadAsync<Form>();
            long[] key = f[nameof(key)];
            using (var dc = ac.NewDbContext())
            {
                if (key != null)
                {
                    dc.Sql("DELETE FROM orders WHERE wx = @1 AND status = @2 AND id")._IN_(key);
                    dc.Execute(p => p.Set(wx).Set(Order.CREATED));
                }
                else
                {
                    dc.Execute("DELETE FROM orders WHERE wx = @1 AND status = @2", p => p.Set(wx).Set(Order.CREATED));
                }
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
                if (dc.Query1("SELECT id, detail, total FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0", p => p.Set(shopid).Set(wx)))
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
                        shop = shopname,
                        buyer = prin.nickname,
                        wx = prin.wx,
                        tel = prin.tel,
                        city = prin.city,
                        distr = prin.distr,
                        addr = prin.addr,
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

    [Ui("当前单")]
    public class MyActiveOrderWork : MyOrderWork<MyActiveOrderVarWork>
    {
        public MyActiveOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1;
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE wx = @1 AND status = @2");
                if (dc.Query(p => p.Set(wx).Set(Order.ACCEPTED)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj, @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, @public: false, maxage: 3);
                }
            }
        }
    }

    [Ui("已往单")]
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
                dc.Sql("SELECT ").columnlst(Order.Empty, proj)._("FROM orders WHERE wx = @1 AND status > @2 ORDER BY id LIMIT 10 OFFSET @3");
                if (dc.Query(p => p.Set(wx).Set(Order.ACCEPTED).Set(page * 10)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj, @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, @public: false, maxage: 3);
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
                    ac.GiveGridPage(200, dc.ToDatas<Order>(proj), proj, @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, @public: false, maxage: 3);
                }
            }
        }
    }

    [Ui("购物车")]
    [User(User.AID)]
    public class OprCartOrderWork : OprOrderWork<OprCartOrderVarWork>
    {
        public OprCartOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.CREATED;
            proj = -1 ^ Order.LATE ^ Order.WX;
        }

        [Ui("一周清理", "清理一周以前的旧单", Mode = UiMode.ButtonConfirm)]
        public void clear(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE shopid = @1 AND status = 0 AND age(created) > '7 days'", p => p.Set(shopid));
                ac.GiveRedirect();
            }
        }
    }

    [Ui("当前单")]
    [User(User.AID)]
    public class OprActiveOrderWork : OprOrderWork<OprActiveOrderVarWork>
    {
        public OprActiveOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.ACCEPTED;
            proj = -1 ^ Order.WX;
        }

        [Ui("发送通知", Mode = UiMode.ButtonShow)]
        public void shipped(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];

            if (ac.GET)
            {
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                    dc.Execute();
                }
            }
            ac.GiveRedirect();
        }

        [Ui("委托办理", Mode = UiMode.ButtonShow)]
        public async Task calc(ActionContext ac)
        {
            string shopid = ac[-1];
            long[] pk = ac.Query[nameof(pk)];

            if (ac.GET)
            {
            }
            else
            {
                var key = await ac.ReadAsync<Form>();
                using (var dc = ac.NewDbContext())
                {
                }
            }
        }
    }

    [Ui("已往单")]
    [User(User.AID)]
    public class OprPastOrderWork : OprOrderWork<OprPastOrderVarWork>
    {
        public OprPastOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.ABORTED;
            status2 = Order.SHIPPED;
            proj = -1 ^ Order.LATE ^ Order.WX;
        }

        [Ui("查询")]
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


    [Ui("受托单", "代派别家的订单")]
    [User(User.AID)]
    public class OprPartnerOrderWork : OrderWork<OprPartnerOrderVarWork>
    {
        public OprPartnerOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = @2 ORDER BY id LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Order>(), @public: false, maxage: 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, @public: false, maxage: 3);
                }
            }
        }
    }
}