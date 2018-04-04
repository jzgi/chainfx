using System;
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

        // for customer side viewing
        protected void GiveBoardOrderPage(WebContext wc, Order[] arr, bool tools = true)
        {
            wc.GivePage(200, h =>
            {
                if (tools)
                {
                    h.TOOLBAR();
                }
                h.BOARDVIEW(arr,
                    o => { h.H5(o.orgname); },
                    o =>
                    {
                        h.P_("收货").T(o.custaddr)._T(o.custname).T(o.custtel)._P();

                        h.P("品名", wid: 0x12).P("单价", wid: 0x16).P("购量", wid: 0x16).P("到货", wid: 0x16);
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            if (o.status <= 1)
                            {
                                h.P(oi.name, wid: 0x12).P(oi.price, wid: 0x16).P_(wid:0x16).LINK_(nameof(MyOrderVarWork.Upd), i).T(oi.qty)._LINK()._P().P(oi.load, wid: 0x16);
                            }
                            else
                            {
                                h.P_().T(oi.name)._T("¥").T(oi.price)._T(oi.qty).T(oi.unit)._P();
                            }
                        }
                        h.P_("总计").T("¥").T(o.total)._P();
                    },
                    tools ? o => h.TOOLPAD() : (Action<Order>) null
                );
            }, false, 2);
        }

        // for org side viewing
        protected void GiveAccordionOrderPage(WebContext wc, Order[] arr, bool tools = true)
        {
            wc.GivePage(200, h =>
            {
                if (tools)
                {
                    h.TOOLBAR();
                }
                h.ACCORDIONVIEW(arr,
                    o => { h.T(o.custname); },
                    o =>
                    {
                        h.P_("收货").T(o.custname)._T(o.custaddr).T(o.custtel)._P();
                        h.P("品名", wid: 0x12).P("单价", wid: 0x16).P("购量", wid: 0x16).P("到货", wid: 0x16);
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            if (o.status <= 1)
                            {
                                h.P(oi.name, wid: 0x12).P(oi.price, wid: 0x16).P(oi.qty,wid:0x16).P(oi.load, wid: 0x16);
                            }
                            else
                            {
                                h.P_().T(oi.name)._T("¥").T(oi.price)._T(oi.qty).T(oi.unit)._P();
                            }
                        }
                        h.P_("总计").T("¥").T(o.total)._P();
                        h.TOOLPAD();
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
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 0 AND 1 AND custwx = @1 ORDER BY id DESC", p => p.Set(wx));
                GiveBoardOrderPage(wc, arr);
            }
        }

        [Ui("已往订单"), Tool(ButtonOpen)]
        public void old(WebContext wc, int page)
        {
            string wx = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status >= 2 AND custwx = @1 ORDER BY id DESC", p => p.Set(wx));
                GiveBoardOrderPage(wc, arr, false);
            }
        }
    }

    [Ui("新订单")]
    public class OprNewoWork : OrderWork<OprNewoVarWork>
    {
        public OprNewoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status BETWEEN 0 AND 1 AND orgid = @1 ORDER BY id DESC", p => p.Set(orgid));
                GiveAccordionOrderPage(wc, dc.ToArray<Order>());
            }
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
                GiveAccordionOrderPage(wc, arr);
            }
        }

        [Ui("查询"), Tool(LinkShow)]
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