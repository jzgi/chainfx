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
        protected void GiveBoardPage(WebContext wc, Order[] arr, bool tooling = true)
        {
            wc.GivePage(200, h =>
            {
                if (tooling)
                {
                    h.TOOLBAR(title: "我的订单");
                }
                h.BOARD(arr, o =>
                    {
                        h.T("<header class=\"uk-card-header uk-flex uk-flex-middle\">");
                        h.T("<h5>").T(o.grpid).T("</h5>").BADGE(Statuses[o.status], o.status == 0 ? Warning : o.status == 1 ? Success : None);
                        h.T("</header>");

                        h.UL_("uk-list uk-list-divider uk-card-body");

                        h.LI("收　货", o.uaddr, o.uname, o.utel);
                        h.LI_();
                        h.UL_("uk-list uk-list-divider uk-width-1-1");
                        h.LI_();
                        h.ICO_(css: "uk-width-1-6").T('/').T(o.item).T("/icon")._ICO();
                        if (o.status == ORD_CREATED)
                        {
                            h.FI(null, o.item).P_(w: 0x13).CUR(o.price).T("／").T(o.unit)._P();
                            h.T("<p class=\"uk-width-1-6 \">").TOOL(nameof(MyOrderVarWork.cancel), 0, o.qty.ToString())._P();
                        }
                        else
                        {
                            h.FI(null, o.item).P_(w: 0x16).CUR(o.price)._P().P_(w: 0x16).T(o.qty).T(o.unit)._P();
                        }
                        h._LI();
                        h._UL();
                        h._LI();
                        h.LI_("总　额").CUR(o.total)._LI();
                        h._UL(); // uk-card-body

                        if (tooling) h.VARTOOLS(css: "uk-card-footer");
                    }
                );
            }, false, 2);
        }

        // for org side viewing
        protected void GiveAccordionPage(WebContext wc, Order[] arr, bool tools = true)
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
                        h.T("<h4 class=\"uk-width-expand\">").T(o.uname).T("</h4>").BADGE(Statuses[o.status], o.status == 0 ? Warning : o.status == 1 ? Success : None);
                        h.T("</section>");

                        h.T("<section class=\"uk-accordion-content uk-grid\">");
                        h.P_("收　货").SP().T(o.uname).SP().T(o.uaddr).SP().T(o.utel)._P();
                        h.UL_("uk-grid");
                        h.LI_();
                        h.SPAN_(0x11).T(o.item)._SPAN().SPAN_(w: 0x23).T('￥').T(o.price).T("／").T(o.unit)._SPAN().SPAN(o.qty, w: 0x13);
                        h._LI();
                        h._UL();
                        h.P_("总　额", w: 0x12).CUR(o.total)._P();

                        h.VARTOOLS();

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
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 0 AND 1 AND uid = @1 ORDER BY id DESC", p => p.Set(myid));
                wc.GivePage(200, h =>
                {
                    h.BOARD(arr, o =>
                        {
                            h.UL_("uk-list uk-list-divider uk-card-body");
                            h.LI("收货", o.uaddr, o.uname, o.utel);
                            h.LI_();
                            h.ICO_(css: "uk-width-1-3").T('/').T(o.item).T("/icon")._ICO();
                            h.FI(null, o.item).P_(w: 0x16).CUR(o.price)._P().P_(w: 0x16).T(o.qty).T(o.unit)._P();
                            h._LI();
                            h._UL(); // uk-card-body

                            h.VARTOOLS(css: "uk-card-footer");
                        }
                    );
                }, false, 2, title: "我的订单");
            }
        }

        [Ui("查看历史订单"), Tool(AOpen, size: 2)]
        public void old(WebContext wc, int page)
        {
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status >= 2 AND custid = @1 ORDER BY id DESC", p => p.Set(myid));
                GiveBoardPage(wc, arr, false);
            }
        }
    }

    [Ui("订购"), UserAccess(CTR_MGR)]
    public class CtrOrderWork : OrderWork<CtrOrderVarWork>
    {
        public CtrOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 1 AND 4 ORDER BY id");
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("撤消", "确认要撤销此单吗？实收款项将退回给买家"), Tool(ButtonPickConfirm)]
        public async Task abort(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            short rev = 0;
            decimal cash = 0;
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT rev, cash FROM orders WHERE id = @1 AND status = 1", p => p.Set(orderid)))
                {
                    dc.Let(out rev).Let(out cash);
                }
            }
            if (cash > 0)
            {
                string err = await ((SampService) Service).WeiXin.PostRefundAsync(orderid + "-" + rev, cash, cash);
                if (err == null) // success
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = -1, aborted = localtimestamp WHERE id = @1 AND orgid = @2", p => p.Set(orderid).Set(orgid));
                    }
                }
            }
            wc.GiveRedirect("../");
        }
    }

    /// <summary>
    /// The order workset as the <code>supplier</code> role
    /// </summary>
    [Ui("供应"), UserAccess(CTR_SPR)]
    public class SprOrderWork : OrderWork<SprOrderVarWork>
    {
        public SprOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            var prin = (User) wc.Principal;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT id, item, qty, status FROM orders WHERE status BETWEEN 1 AND 2 AND item IN (SELECT name FROM items WHERE supplierid = @1) ORDER BY item, id", p => p.Set(prin.id));

                    string curitme = null; 
                    while (dc.Next())
                    {
                        dc.Let(out int id).Let(out string item).Let(out short qty).Let(out short status);

                        if (item != curitme)
                        {
                            if (curitme != null)
                            {
                                h.T("</main>");
                                h.TOOLS(css: "uk-card-footer");
                                h.T("</article>");
                            }
                            h.T("<article class=\"uk-card  uk-card-default\">");
                            h.T("<header class=\"uk-card-header\">").T(item).T("</header>");
                            h.T("<main class=\"uk-card-body\">");
                        }

                        h.T("<input type=\"checkbox\" name=\"").T(id).T("\"").T(" checked", status == 2).T("><label for=\"").T(id).T(" class=\"checkable\">").T(qty).T("</label>");

                        curitme = item;
                    }
                    h.T("</main>");
                    h.TOOLS(css: "uk-card-footer");
                    h.T("</article>");
                }
            }, false, 2);
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
                    dc.Sql("UPDATE orders SET status = ").T(ORD_PAID).T(" WHERE status > ").T(ORD_PAID).T(" AND orgid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(orgid), prepare: false);
                }
            }
            wc.GiveRedirect();
        }
    }

    /// <summary>
    /// The order workset as the <code>deliverer</code> role
    /// </summary>
    [Ui("派送"), UserAccess(CTR_DVR)]
    public class DvrOrderWork : OrderWork<DvrOrderVarWork>
    {
        public DvrOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
        }
    }

    [Ui("订购")]
    public class GrpOrderWork : OrderWork<GrpOrderVarWork>
    {
        public GrpOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("当前"), Tool(A)]
        public void @default(WebContext wc)
        {
            string grpid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 1 AND 4 AND grpid = @1 ORDER BY id", p => p.Set(grpid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("查找"), Tool(APrompt)]
        public void find(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            string tel = null;
            if (inner)
            {
                wc.GivePane(200, h => { h.FORM_().FIELDSET_("手机号").TEL(nameof(tel), tel)._FIELDSET()._FORM(); });
            }
            else
            {
                string grpid = wc[-1];
                tel = wc.Query[nameof(tel)];
                using (var dc = NewDbContext())
                {
                    var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 1 AND 4 AND utel = @1", p => p.Set(tel));
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR(title: tel);
                        h.TABLE(arr, null,
                            o => h.TD(o.utel, o.uname).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                        );
                    });
                }
            }
        }

        [Ui("收货"), Tool(ButtonPickPrompt)]
        public async Task receive(WebContext wc)
        {
            string grpid = wc[-1];
            if (wc.GET)
            {
                int[] key = wc.Query[nameof(key)];
                wc.GivePane(200, h =>
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT item, SUM(qty) AS num FROM orders WHERE id")._IN_(key).T(" AND status = 3 AND grpid = @1 GROUP BY item");
                        dc.Query(p => p.SetIn(key).Set(grpid));
                        h.FORM_();

                        h.T("仅列出已送达货品");
                        h.T("<table class=\"uk-table\">");
                        while (dc.Next())
                        {
                            dc.Let(out string item).Let(out short num);
                            h.TD(item).TD(num);
                        }
                        h.T("</table>");
                        h.CHECKBOX("", false, "我确认收货", required: true);
                        h._FORM();
                    }
                });
            }
            else // POST
            {
                int[] key = (await wc.ReadAsync<Form>())[nameof(key)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = 4 WHERE id")._IN_(key).T(" AND status = 3 AND grpid = @1");
                    dc.Execute(p => p.SetIn(key).Set(grpid));
                }
                wc.GiveRedirect();
            }
        }

        [Ui("递货"), Tool(ButtonPickPrompt)]
        public void give(WebContext wc)
        {
        }

        [Ui("查历史"), Tool(AOpen, size: 4)]
        public void history(WebContext wc)
        {
        }
    }
}