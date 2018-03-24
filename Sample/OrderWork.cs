using System;
using System.Net.Security;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Core.Order;
using static Core.User;

namespace Core
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, long>((obj) => ((Order) obj).id);
        }

        protected void PrinOrders(Order[] arr, WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.BOARDVIEW(arr,
                    o => { h.T("No.").T(o.id).SEP().T(o.paid); },
                    o =>
                    {
                        h.P_("收货").T(o.name)._T(o.addr)._T(o.tel)._P();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.P(oi.name, width: 6).P_(width: 0x23).T("¥").T(oi.price)._P().P_(width: 0x1).T(oi.qty)._P();
                        }
                        h.P_("总价").T("¥").T(o.total)._P();
                    });
            }, false, 2);
        }

        protected void PrintOrdersPage2(WebContext wc, Order[] arr)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.BOARDVIEW(arr,
                    o => { h.T(o.orgname)._IF(o.paid); },
                    o =>
                    {
                        h.FIELD_("收货").T(o.addr)._T(o.name).T(o.tel)._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            if (o.status <= 1)
                            {
                                h.ICON("/org/" + o.orgid + "/" + oi.name + "/icon", width: 1);
                                h.BOX_(3).P(oi.name).P(oi.price).P(oi.qty)._BOX();
                                h.TOOL(nameof(MyOrderVarWork.edit));
                                h.BOX_(1);
                                if (o.typ == POS)
                                {
                                    h.P(oi.load);
                                }
                                h._BOX();
                            }
                            else
                            {
                                h.FIELD_().T(oi.name)._T("¥").T(oi.price)._T(oi.qty).T(oi.unit)._FIELD();
                            }
                        }
                        h.P_("总计").T("¥").T(o.total)._P();
                    });
            }, false, 2);
        }
    }

    public class MyOrderWork : OrderWork<MyOrderVarWork>
    {
        public MyOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string wx = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE wx = @1 AND status <= 1 ORDER BY id DESC", p => p.Set(wx));
            }
        }

        [Ui("历史订单"), Tool(ButtonOpen)]
        public void old(WebContext wc, int page)
        {
            string wx = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE wx = @1 AND status > 1 ORDER BY id DESC", p => p.Set(wx));
                PrintOrdersPage2(wc, arr);
            }
        }
    }

    [Ui("销售点管理")]
    public class OprPosWork : OrderWork<OprPosVarWork>
    {
        public OprPosWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status = 0 AND orgid = @1 AND typ = 1", p => p.Set(orgid));
                PrintOrdersPage2(wc, arr);
            }
        }

        [Ui("新建"), Tool(ButtonConfirm), User(OPRSTAFF)]
        public void @new(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var org = Obtain<Map<string, Org>>()[orgid];
                var o = new Order
                {
                    rev = 1,
                    status = 0,
                    orgid = orgid,
                    orgname = org.name,
                    typ = POS,
                    created = DateTime.Now
                };
                const byte proj = 0xff ^ KEY ^ Order.LATER;
                dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                dc.Execute(p => o.Write(p, proj), false);
            }
            wc.GiveRedirect();
        }

        [Ui("删除"), Tool(ButtonPickConfirm), User(OPRSTAFF)]
        public async Task del(WebContext wc, int page)
        {
            string orgid = wc[-1];
            int[] key = (await wc.ReadAsync<Form>())[nameof(key)];
            if (key != null)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("DELETE FROM orders WHERE orgid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(orgid), false);
                }
            }
            wc.GiveRedirect();
        }
    }

    [Ui("新订单")]
    public class OprNewoWork : OrderWork<OprNewoVarWork>
    {
        public OprNewoWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("全部"), Tool(Anchor)]
        public void @default(WebContext wc, int page)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status = " + PAID + " AND orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
//                    PrintOrdersPage2(wc, arr);
            }
        }

        [Ui("按区域"), Tool(AnchorPrompt)]
        public void area(WebContext wc, int page)
        {
            string orgid = wc[-1];
            bool inner = wc.Query[nameof(inner)];
            string filter = (string) wc.Query[nameof(filter)] ?? string.Empty;
            if (inner)
            {
                wc.GivePane(200, m =>
                {
                    var org = Obtain<Map<string, Org>>()[orgid];
                    m.FORM_();
//                    m.RADIOSET(nameof(filter), filter, org.areas);
                    m._FORM();
                });
                return;
            }
            wc.GivePage(200, h =>
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT * FROM orders WHERE status = " + PAID + " AND orgid = @1 AND addr LIKE @2 ORDER BY id DESC LIMIT 20 OFFSET @3", p => p.Set(orgid).Set(filter + "%").Set(page * 20));
                }
            }, false, 3);
        }

        static readonly Map<string, string> MSGS = new Map<string, string>
        {
            ["订单处理"] = "我们已经接到您的订单（金额{0}元）",
            ["派送通知"] = "销售人员正在派送您所购的商品",
            ["sdf"] = "",
        };

        [Ui("通知"), Tool(ButtonPickShow)]
        public void send(WebContext wc)
        {
            long[] key = wc.Query[nameof(key)];
            string msg = null;
            if (wc.GET)
            {
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.RADIOSET(nameof(msg), msg, MSGS, "消息通知买家", width: 0x4c);
                    m._FORM();
                });
            }
            else
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT wx FROM orders WHERE id")._IN_(key);
                    dc.Execute(prepare: false);
                }
                wc.GivePane(200);
            }
        }
    }

    [Ui("旧订单"), User(OPR)]
    public class OprOldoWork : OrderWork<OprOldoVarWork>
    {
        public OprOldoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status > 4 AND orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                PrinOrders(arr, wc);
            }
        }

        [Ui("查询"), Tool(AnchorShow)]
        public void send(WebContext wc)
        {
            long[] key = wc.Query[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            wc.GiveRedirect();
        }

        [Ui("回退", "【警告】把选中的订单回退成新单？"), Tool(ButtonPickConfirm)]
        public async Task back(WebContext wc)
        {
            string orgid = wc[-2];
            var f = await wc.ReadAsync<Form>();
            string[] key = f[nameof(key)];
            if (key != null)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = ").T(PAID).T(" WHERE status > ").T(PAID).T(" AND orgid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(orgid), prepare: false);
                }
            }
            wc.GiveRedirect();
        }
    }
}