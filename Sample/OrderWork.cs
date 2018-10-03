using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Order;

namespace Samp
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, int>((obj) => ((Order) obj).id);
        }
    }

    public class MyOrderWork : OrderWork<MyOrderVarWork>
    {
        public MyOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string hubid = wc[0];
            int uid = wc[Parent];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE uid = @1 ORDER BY id DESC", p => p.Set(uid));
                wc.GivePage(200, h =>
                {
                    h.BOARD(arr, o =>
                        {
                            h.HEADER_("uk-card-header");
                            h.T("收货：").T(o.uaddr).SP().T(o.uname).SP().T(o.utel);
                            h._HEADER();
                            h.MAIN_("uk-card-body uk-row");
                            h.ICO_(css: "uk-width-1-6").T("/").T(hubid).T("/").T(o.itemid).T("/icon")._ICO();
                            h.DIV_("uk-width-2-3").SP().T(o.itemname).SP().CNY(o.price).T(o.qty).T("/").T(o.unit)._DIV();
                            h.VARTOOLPAD(css: "uk-width-1-6");
                            h._MAIN();
                        }
                    );
                }, false, 3, title: "我的订单", refresh: 120);
            }
        }
    }

    [UserAccess(hubly: 7)]
    [Ui("订单")]
    public class HubOrderWork : OrderWork<HubOrderVarWork>
    {
        public HubOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("新收款"), Tool(Anchor, "uk-button-link")]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdPaid).T(" AND hubid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("已交货"), Tool(Anchor, "uk-button-link")]
        public void given(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdGiven).T(" AND hubid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("货到中库"), Tool(Anchor, "uk-button-link")]
        public void taken(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdTaken).T(" AND hubid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("货已派"), Tool(Anchor, "uk-button-link")]
        public void sent(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdSent).T(" AND hubid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("历史"), Tool(Anchor)]
        public void shipped(WebContext wc)
        {
        }
    }

    /// <summary>
    /// The order workset as the <code>supplier</code> role
    /// </summary>
    [Ui("订单")]
    public class ShopOrderWork : OrderWork<ShopOrderVarWork>
    {
        public ShopOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("新订单"), Tool(Anchor, "uk-button-link")]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-2];
            var prin = (User) wc.Principal;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, itemname, SUM(qty), unit FROM orders WHERE status =").T(OrdPaid).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @1) GROUP BY itemid, itemname, unit");
                    dc.Query(p => p.Set(orgid));
                    string curitme = null;
                    while (dc.Next())
                    {
                        dc.Let(out int id).Let(out string item).Let(out short qty).Let(out short unit);

                        if (item != curitme)
                        {
                            if (curitme != null)
                            {
                                h.T("</main>");
                                h.TOOLPAD(group: 2, css: "uk-card-footer");
                                h.T("</form>");
                            }
                            h.T("<form class=\"uk-card uk-card-default\">");
                            h.T("<header class=\"uk-card-header\">").T(item).T("（").T(unit).T("）</header>");
                            h.T("<main class=\"uk-card-body\">");
                        }

                        curitme = item;
                    }
                    h.T("</main>");
                    h.TOOLPAD(group: 0b0110, css: "uk-card-footer uk-flex-between");
                    h.T("</form>");
                }
            }, false, 2);
        }

        [Ui("概况", group: 1), Tool(ButtonPickShow)]
        public void summary(WebContext wc)
        {
            bool range = true;
        }


        [Ui("排程", "设为排程状态", 0b0010), Tool(ButtonPickShow, css: "uk-button-secondary")]
        public void plan(WebContext wc)
        {
            bool range = true;
            if (wc.GET)
            {
                wc.GivePage(200, h =>
                {
                    h.FORM_().FIELDUL_("个别选择还是区间选择");
                    h.CHECKBOX(nameof(range), range, "选择连续区间");
                    h._FIELDUL()._FORM();
                });
            }
            else
            {
                int[] key = wc.Query[nameof(key)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                    dc.Execute();
                }
                wc.GiveRedirect();
            }
        }

        [Ui("解排", "解除排程状态", 0b0010), Tool(ButtonPickShow, css: "uk-button-secondary")]
        public async Task unplan(WebContext wc)
        {
            bool range = false;
            if (wc.GET)
            {
                wc.GivePage(200, h =>
                {
                    h.FORM_().FIELDUL_("是特定选择还是区间选择");
                    h.CHECKBOX(nameof(range), range, "选择连续区间");
                    h._FIELDUL()._FORM();
                });
            }
            else
            {
                int[] key = wc.Query[nameof(key)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                    dc.Execute();
                }
                wc.GiveRedirect();
            }
        }

        [Ui("备齐", "解除排程状态", 0b0100), Tool(ButtonPickShow, css: "uk-button-secondary")]
        public async Task ready(WebContext wc)
        {
        }
    }

    [Ui("订单")]
    public class TeamOrderWork : OrderWork<TeamOrderVarWork>
    {
        public TeamOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("货到团"), Tool(Anchor, "uk-button-link")]
        public void @default(WebContext wc)
        {
            short teamid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdReceived).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("货到中库"), Tool(Anchor, "uk-button-link")]
        public void way(WebContext wc)
        {
            short teamid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdTaken).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("货未妥"), Tool(Anchor, "uk-button-link")]
        public void not(WebContext wc)
        {
            short teamid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status BETWEEN ").T(OrdPaid).T(" AND ").T(OrdGiven).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("查找"), Tool(AnchorPrompt, "uk-button-link")]
        public void find(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            string tel = null;
            if (inner)
            {
                wc.GivePane(200, h => { h.FORM_().FIELDUL_("手机号").TEL(null, nameof(tel), tel)._FIELDUL()._FORM(); });
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
                            o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                        );
                    });
                }
            }
        }
    }
}