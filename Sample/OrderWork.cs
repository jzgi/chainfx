using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Order;
using static Samp.User;
using static Greatbone.Style;

namespace Samp
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, int>((obj) => ((Order) obj).id);
        }

        // for customer side viewing
        protected void GiveBoardOrderPage(WebContext wc, Order[] arr, bool tooling = true)
        {
            wc.GivePage(200, h =>
            {
                if (tooling)
                {
                    h.TOOLBAR();
                }
                h.BOARD(arr, o =>
                    {
                        h.T("<section class=\"uk-card-header uk-flex uk-flex-middle\">");
                        h.T("<h3>").T(o.orgname).T("</h3>").BADGE(Statuses[o.status], o.status == 0 ? Warning : o.status == 1 ? Success : None);
                        h.T("</section>");

                        h.UL_("uk-card-body");

                        h.LI("收货", o.custaddr, o.custname, o.custtel);

                        h.UL_("uk-grid");
                        h.LI_().LABEL("品名", 0x12).LABEL("单价", 0x16).LABEL("购量", 0x16).LABEL("到货", 0x16)._LI();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            if (o.status == CREATED)
                            {
                                h.P(oi.name, w: 0x12).P_(w: 0x16).CUR(oi.price)._P().P_(w: 0x16).TOOL(nameof(MyOrderVarWork.Upd), i, oi.qty.ToString())._P().P(oi.ship, w: 0x16);
                            }
                            else
                            {
                                h.P(oi.name, w: 0x12).P_(w: 0x16).CUR(oi.price)._P().P_(w: 0x16).T(oi.qty).T(oi.unit)._P();
                            }
                        }
                        h._UL();
                        h.P_("总额", w: 0x12).CUR(o.total)._P();
                        if (o.comp)
                        {
                            h.P_("净额", w: 0x12).CUR(o.net)._P();
                        }
                        h._UL(); // uk-card-body

                        if (tooling) h.TOOLPAD(css: "uk-card-footer");
                    }
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
                h.ACCORDION(arr,
                    o =>
                    {
                        h.T("<section class=\"uk-accordion-title\">");
                        h.T("<h3 class=\"uk-width-expand\">").T(o.custname).T("</h3>").BADGE(Statuses[o.status], o.status == 0 ? Warning : o.status == 1 ? Success : None);
                        h.T("</section>");

                        h.T("<section class=\"uk-accordion-content uk-grid\">");
                        h.P_("收货").T(o.custname).T(o.custaddr).T(o.custtel)._P();
                        h.UL_("uk-grid");
                        h.LI_().LABEL("品名", 0x12).LABEL("单价", 0x16).LABEL("购量", 0x16).LABEL("到货", 0x16)._LI();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.LI_();
                            h.SPAN_(0x12).T(oi.name).T(" (").T(oi.unit).T(")")._SPAN().CUR(oi.price, w: 0x16).SPAN(oi.qty, w: 0x16).NUMIF(oi.ship, w: 0x16);
                            h._LI();
                        }
                        h._UL();
                        h.P_("总额", w: 0x12).CUR(o.total)._P();
                        if (o.comp)
                        {
                            h.P_("净额", w: 0x12).CUR(o.net)._P();
                        }
                        h.TOOLPAD();

                        h.T("</section>");
                    }, null);
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
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 0 AND 1 AND custid = @1 ORDER BY id DESC", p => p.Set(myid));
                GiveBoardOrderPage(wc, arr);
            }
        }

        [Ui("历史记录"), Tool(AOpen)]
        public void old(WebContext wc, int page)
        {
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status >= 2 AND custid = @1 ORDER BY id DESC", p => p.Set(myid));
                GiveBoardOrderPage(wc, arr, false);
            }
        }
    }

    [Ui("新单")]
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
                    m.RADIOSET(nameof(msg), msg, MSGS, "消息通知买家", w: 0x4c);
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

        [Ui("清理", "清理三天以前未付款或者已撤销的订单"), Tool(ButtonConfirm)]
        public void clean(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE (status = 0 OR status = -1) AND orgid = @1 AND (created + interval '3 day' < localtimestamp)", p => p.Set(orgid));
            }
            wc.GiveRedirect();
        }
    }

    [Ui("旧单"), User(OPR)]
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
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status >= 2 AND orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                GiveAccordionOrderPage(wc, arr);
            }
        }

        [Ui("查询"), Tool(AShow)]
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