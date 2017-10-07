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

    [Ui("购物车")]
    public class MyPreWork : OrderWork<MyPreVarWork>
    {
        public MyPreWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE wx = @1 AND status = 0 ORDER BY id DESC");
                if (dc.Query(p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.COL("单号", o.id, 0);
                        h.COL("总价", o.total, 0);
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, null, false, 3);
                }
            }
        }

        public async Task add(ActionContext ac)
        {
            string wx = ac[-1];
            short shopid;
            string name;
            string opt = null;
            string[] sets = null;
            if (ac.GET)
            {
                shopid = ac.Query[nameof(shopid)];
                name = ac.Query[nameof(name)];

                ac.GivePane(200, h =>
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Sql("SELECT ").columnlst(Item.Empty)._("FROM items WHERE shopid = @1 AND name = @2");
                        dc.Query1(p => p.Set(shopid).Set(name));
                        var item = dc.ToObject<Item>();
                        h.FORM_();

                        h.T("<img src=\"/shop/").T(shopid).T("/").T(item.name).T("/icon\">");

                        h.T("<div>").T(item.name).T("</div>");

                        h.T("<div>&yen;").T(item.price).T("</div>");

                        h.NUMBER(nameof(item.max), item.min, min: item.min, step: item.step);

                        if (item.customs != null)
                        {
                            h.CHECKBOXGROUP(nameof(opt), sets, item.customs, "定制要求");
                        }

                        h._FORM();
                    }
                });
                return;
            }

            var f = await ac.ReadAsync<Form>();
            shopid = f[nameof(shopid)];
            string shopname = f[nameof(shopname)];
            name = f[nameof(name)];
            string unit = f[nameof(unit)];
            decimal price = f[nameof(price)];
            short qty = f[nameof(qty)];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT id, detail, total FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0", p => p.Set(shopid).Set(wx)))
                {
                    var o = new Order();
                    dc.Let(out o.id).Let(out o.lines).Let(out o.total);
                    o.AddItem(name, qty, unit, price);
                    o.Sum();
                    dc.Execute("UPDATE orders SET detail = @1, total = @2 WHERE id = @3", p => p.Set(o.lines).Set(o.total).Set(o.id));
                }
                else
                {
                    User prin = (User) ac.Principal;
                    var o = new Order
                    {
                        shopid = shopid,
                        shopname = shopname,
                        name = prin.name,
                        wx = prin.wx,
                        tel = prin.tel,
                        city = prin.city,
                        addr = prin.addr,
                        lines = new[]
                        {
                            new OrderLine {name = name, price = price, qty = qty, unit = unit}
                        },
                        created = DateTime.Now
                    };
                    o.Sum();

                    const short proj = 0x00ff ^ Order.ID;

                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.Give(200);
            }
        }

        [Ui("清空购物车/删除", "清空购物车或者删除选中的项", Mode = UiMode.ButtonConfirm)]
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
    }

    [Ui("我的订单")]
    public class MyOrderWork : OrderWork<MyOrderVarWork>
    {
        public MyOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE wx = @1 AND status > 0 ORDER BY id DESC");
                if (dc.Query(p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.COL("单号", o.id, 0);
                        h.COL("总价", o.total, 0);
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, null, false, 3);
                }
            }
        }
    }

    [Ui("新单")]
    [User(User.OPR_)]
    public class OprNewOrderWork : OrderWork<OprNewOrderVarWork>
    {
        static readonly Map<short, string> NOTIFS = new Map<short, string>()
        {
            [1] = "正在为您的订单作备货生产",
            [2] = "您的订单的备货生产已完成，准备发货",
            [3] = "您的订单已发货，请您准备接收",
            [4] = "您的订单已接收，请您作确认收货操作",
        };

        public OprNewOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            short shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.ACCEPTED + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.COL("单号", o.id, 0);
                        h.COL("总价", o.total, 0);
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
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
                List<(long, string)> rows = new List<(long, string)>(16);
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
                            rows.Add((id, wx));
                        }
                    }
                }

                for (int i = 0; i < rows.Count; i++)
                {
                    (long, string) row = rows[i];
                    await WeiXinUtility.PostSendAsync(row.Item2, "【商家通知】" + NOTIFS[notif] + "（订单编号：" + row.Item1 + "）");
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
                    dc.Execute(@"UPDATE shops SET coshopid = @1 WHERE id = @2", p => p.Set(duo.Item1).Set(shopid));
                }
                ac.GivePane(200);
            }
        }
    }

    [Ui("派单")]
    [User(User.OPR_)]
    public class OprOnOrderWork : OrderWork<OprOnOrderVarWork>
    {
        public OprOnOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            short shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.SHIPPED + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.COL("单号", o.id, 0);
                        h.COL("总价", o.total, 0);
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
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

    [Ui("旧单")]
    [User(User.OPR_)]
    public class OprPastOrderWork : OrderWork<OprPastOrderVarWork>
    {
        public OprPastOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            short shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.RECKONED + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.COL("单号", o.id, 0);
                        h.COL("总价", o.total, 0);
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
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
}