using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Order;
using static Samp.User;

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

        public void @default(WebContext wc)
        {
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 0 AND 4 AND uid = @1 ORDER BY id DESC", p => p.Set(myid));
                wc.GivePage(200, h =>
                {
                    h.BOARD(arr, o =>
                        {
                            h.HEADER_("uk-card-header");
                            h.T("收货：").T(o.uaddr).SP().T(o.uname).SP().T(o.utel);
                            h._HEADER();
                            h.MAIN_("uk-card-body uk-row");
                            h.ICO_(css: "uk-width-1-6").T('/').T(o.item).T("/icon")._ICO();
                            h.DIV_("uk-width-2-3").SP().T(o.item).SP().CNY(o.price).T(o.qty).T("/").T(o.unit)._DIV();
                            h.VARTOOLPAD(css: "uk-width-1-6");
                            h._MAIN();
                        }, css: "uk-card-primary"
                    );
                }, false, 2, title: "我的订单");
            }
        }

        [Ui("查看历史订单"), Tool(AnchorOpen, size: 2)]
        public void old(WebContext wc, int page)
        {
            int myid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status >= 2 AND custid = @1 ORDER BY id DESC", p => p.Set(myid));
//                GiveBoardPage(wc, arr, false);
            }
        }
    }

    [Ui("订单"), UserAccess(RegMgmt)]
    public class HubOrderWork : OrderWork<HubOrderVarWork>
    {
        public HubOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("新收"), Tool(Anchor)]
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

        [Ui("产供"), Tool(Anchor)]
        public void confirmed(WebContext wc)
        {
        }

        [Ui("派送"), Tool(Anchor)]
        public void loaded(WebContext wc)
        {
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

        public void @default(WebContext wc)
        {
            var prin = (User) wc.Principal;
            var items = Obtain<Map<string, Item>>();
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT id, item, qty, unit, status FROM orders WHERE status BETWEEN 1 AND 2 AND item IN (SELECT name FROM items WHERE giverid = @1) ORDER BY item, id", p => p.Set(prin.id));
                    string curitme = null;
                    while (dc.Next())
                    {
                        dc.Let(out int id).Let(out string item).Let(out short qty).Let(out short unit).Let(out short status);

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

                        h.T("<label class=\"checkable\"><input type=\"checkbox\" name=\"").T(id).T("\"").T("><span").T(" class=\"uk-active\"", status == 2).T(">").T(qty).T("</span></label>");

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

        [Ui("到货"), Tool(Anchor, "uk-button-link")]
        public void @default(WebContext wc)
        {
            string teamid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    var arr = dc.Query<Order>("SELECT * FROM orders WHERE status = 5 AND teamid = @1 ORDER BY id", p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("在途"), Tool(Anchor, "uk-button-link")]
        public void way(WebContext wc)
        {
            string teamid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    var arr = dc.Query<Order>("SELECT * FROM orders WHERE status BETWEEN 1 AND 4 AND teamid = @1 ORDER BY id", p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
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
                            o => h.TD(o.utel, o.uname).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                        );
                    });
                }
            }
        }
    }
}