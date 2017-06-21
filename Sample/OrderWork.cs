using System;
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
                const ushort proj = Order.ID | Order.BASIC_DETAIL;
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
                    var o = new Order();
                    dc.Let(out o.id).Let(out o.detail).Let(out o.total);
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

                    const ushort proj = 0x00ff ^ Order.ID;

                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
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
                const ushort proj = Order.ID | Order.BASIC_DETAIL | Order.CASH | Order.FLOW;
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
                const ushort proj = Order.ID | Order.BASIC_DETAIL | Order.CASH | Order.FLOW;
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

        protected ushort proj;

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
            proj = Order.ID | Order.BASIC_DETAIL | Order.CASH;
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
        static readonly Opt<short> NOTIFS = new Opt<short>()
        {
            [1] = "正在为您的订单作备货生产",
            [2] = "您的订单的备货生产已完成，准备发货",
            [3] = "您的订单已发货，请您准备接收",
            [4] = "您的订单已接收，请您作确认收货操作",
        };

        public OprActiveOrderWork(WorkContext wc) : base(wc)
        {
            status = Order.ACCEPTED;
            proj = Order.ID | Order.BASIC_DETAIL | Order.CASH | Order.FLOW;
        }

        [Ui("通知买家", Mode = UiMode.ButtonShow)]
        public async Task sendnotif(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];
            short notif = 0;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    if (key == null)
                    {
                        m.CALLOUT("请先选择目标订单");
                    }
                    else
                    {
                        m.FORM_();
                        m.RADIOS(nameof(notif), notif, NOTIFS, label: "通知内容", required: true);
                        m._FORM();
                    }
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                notif = f[nameof(notif)];
                List<Dual<long, string>> rows = new List<Dual<long, string>>(16);
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT id, wx FROM orders WHERE id")._IN_(key);
                    if (dc.Query())
                    {
                        while (dc.Next())
                        {
                            long id;
                            string wx;
                            dc.Let(out id).Let(out wx);
                            rows.Add(new Dual<long, string>(id, wx));
                        }
                    }
                }

                for (int i = 0; i < rows.Count; i++)
                {
                    Dual<long, string> row = rows[i];
                    await WeiXinUtility.PostSendAsync(row.B, "【商家通知】" + NOTIFS[notif] + "（订单编号：" + row.A + "）");
                }

                ac.GivePane(200);
            }
        }

        [Ui("委托办理", Mode = UiMode.ButtonShow)]
        public async Task passon(ActionContext ac)
        {
            var prin = (User) ac.Principal;
            string shopid = ac[-1];
            string city = prin.city;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        if (dc.Query("SELECT id, name FROM shops WHERE city = @1", p => p.Set(city)))
                        {
                            while (dc.Next())
                            {
                                string id;
                                string name;
                                dc.Let(out id).Let(out name);
                                m.RADIO("id_name", id, name, null, false, id, name, null);
                            }
                            m._FORM();
                        }
                    }
                });
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                string id_name = f[nameof(id_name)];
                var duo = id_name.ToDual<string, string>();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(@"UPDATE shops SET coshopid = @1 WHERE id = @2", p => p.Set(duo.A).Set(shopid));
                }
                ac.GivePane(200);
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
            status2 = Order.RECKONED;
            proj = Order.ID | Order.BASIC_DETAIL | Order.CASH | Order.FLOW;
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


    [Ui("受托单", "受托办理其他商家的订单")]
    [User(User.AID)]
    public class OprCoOrderWork : OrderWork<OprCoOrderVarWork>
    {
        public OprCoOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE coshopid = @1 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
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