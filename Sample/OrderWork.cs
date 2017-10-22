using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;
using static Greatbone.Sample.User;

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
//                    var areas = ((SampleService)Service).Cities[""] 
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION(o.shopname);
                        h.FIELD_("收货地址").T(o.name, o.tel, o.addr, null).BR().BUTTON("设置收货地址")._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var item = o.items[i];
                            h.FIELD_(6).T("<img src=\"\">")._FIELD().FIELD_(6).T(item.qty)._FIELD();
                        }
                        h.FIELD(o.total, "总价", 0);
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
            var f = await ac.ReadAsync<Form>();
            string city = f[nameof(city)];
            string area = f[nameof(area)];
            short shopid = f[nameof(shopid)];
            string shopname = f[nameof(shopname)];
            string name = f[nameof(name)];
            decimal price = f[nameof(price)];
            short qty = f[nameof(qty)];
            string unit = f[nameof(unit)];
            string[] customs = f[nameof(customs)];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT id, items, total FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0", p => p.Set(shopid).Set(wx)))
                {
                    var o = new Order();
                    dc.Let(out o.id).Let(out o.items).Let(out o.total);
                    o.AddItem(name, price, qty, unit, customs);
                    o.SetTotal();
                    dc.Execute("UPDATE orders SET items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                else
                {
                    User prin = (User) ac.Principal;
                    var o = new Order
                    {
                        shopid = shopid,
                        shopname = shopname,
                        wx = prin.wx,
                        name = prin.name,
                        tel = prin.tel,
                        city = city ?? prin.city,
                        region = area ?? prin.area,
                        addr = prin.addr,
                        items = new[] {new OrderItem {name = name, price = price, qty = qty, unit = unit, customs = customs}},
                        created = DateTime.Now
                    };
                    o.SetTotal();
                    const short proj = -1 ^ Order.ID ^ Order.LATER;
                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.Give(200);
            }
        }

        [Ui("清空购物车", Mode = ButtonConfirm)]
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
                        h.FIELD(o.id, "单号", 0);
                        h.FIELD(o.total, "总价", 0);
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
    [User(OPR_)]
    public class OprNewWork : OrderWork<OprNewVarWork>
    {
        static readonly Map<short, string> NOTIFS = new Map<short, string>()
        {
            [1] = "您的订单已收到。",
            [2] = "您的订单已开始备货生产。",
            [3] = "您的订单备货生产已完成，准备派送。",
            [4] = "您的订单已派送完成，如有疑问请与我们联系。",
        };

        public OprNewWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            short shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.PAID + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.FIELD(o.id, "单号", 0);
                        h.FIELD(o.total, "总价", 0);
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
        }

        [Ui("通知买家", Mode = ButtonShow)]
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

        [Ui("委托办理", Mode = ButtonShow)]
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
    [User(OPR_)]
    public class OprGoWork : OrderWork<OprGoVarWork>
    {
        public OprGoWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            short shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.RECEIVED + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.FIELD(o.id, "单号", 0);
                        h.FIELD(o.total, "总价", 0);
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
    [User(OPR_)]
    public class OprPastWork : OrderWork<OprPastVarWork>
    {
        public OprPastWork(WorkContext wc) : base(wc)
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
                        h.FIELD(o.id, "单号", 0);
                        h.FIELD(o.total, "总价", 0);
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

    [Ui("投诉")]
    [User(adm: true)]
    public class AdmKickWork : OrderWork<AdmKickVarWork>
    {
        public AdmKickWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
        }
    }
}