using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Order;

namespace Samp
{
    public abstract class OrderWork : Work
    {
        protected OrderWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MyOrderWork : OrderWork
    {
        public MyOrderWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<MyOrderVarWork>();
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
                            h.VARTOOLS(css: "uk-width-1-6");
                            h._MAIN();
                        }
                    );
                }, false, 3, title: "我的订单", refresh: 120);
            }
        }
    }

    [UserAccess(hubly: 1)]
    [Ui("订单")]
    public class HubOrderWork : OrderWork
    {
        public HubOrderWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<HubOrderVarWork>();
        }

        const int PageSiz = 30;

        [Ui("排队", group: 1), Tool(Anchor)]
        public void @default(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdPaid).T(" AND hubid = @1 ORDER BY id LIMIT ").T(PageSiz).T(" OFFSET @2");
                    var arr = dc.Query<Order>(p => p.Set(hubid).Set(page * PageSiz));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("备货", group: 1), Tool(Anchor)]
        public void given(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdAccepted).T(" AND hubid = @1 ORDER BY id LIMIT ").T(PageSiz).T(" OFFSET @2");
                    var arr = dc.Query<Order>(p => p.Set(hubid).Set(page * PageSiz));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("中转", group: 1), Tool(Anchor)]
        public void taken(WebContext wc)
        {
            string hubid = wc[0];
            var orgs = Obtain<Map<short, Team>>();
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT teamid, itemid, last(itemname), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status =").T(OrdStocked).T(" GROUP BY teamid, itemid");
                    dc.Query(p => p.Set(hubid));
                    while (dc.Next())
                    {
                        h.T("<form class=\"uk-card uk-card-default\">");
                        h.MAIN_("uk-card-body");
                        dc.Let(out short teamid).Let(out short itemid).Let(out string itemname).Let(out short qty).Let(out string unit);
                        h.T(orgs[teamid]?.name).SP().T(itemname).T("：").T(qty).SP().T(unit);
                        h._MAIN();
                        h.TOOLS(group: 2, css: "uk-card-footer uk-flex-center");
                        h.T("</form>");
                    }
                }
            });
        }

        [Ui("派运", group: 2), Tool(Anchor)]
        public void send(WebContext wc, int page)
        {
        }

        [Ui("派运", group: 1), Tool(Anchor)]
        public void sent(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
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

        [Ui("运达", group: 1), Tool(Anchor)]
        public void received(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
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

        [Ui(icon: "search", tip: "查找", group: 1), Tool(AnchorPrompt)]
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

    /// <summary>
    /// The order processing in a workshop.
    /// </summary>
    [Ui("订单")]
    public class ShopOrderWork : OrderWork
    {
        public ShopOrderWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<ShopOrderVarWork>();
        }

        [Ui("排队", group: 1), Tool(Anchor)]
        public void not(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(itemname), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status =").T(OrdPaid).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @2) GROUP BY itemid");
                    dc.Query(p => p.Set(hubid).Set(orgid));
                    while (dc.Next())
                    {
                        h.T("<form class=\"uk-card uk-card-default\">");
                        h.MAIN_("uk-card-body");
                        dc.Let(out short itemid).Let(out string itemname).Let(out short qty).Let(out string unit);
                        h.T(itemname).T("：").T(qty).SP().T(unit);
                        h._MAIN();
                        h.TOOLS(group: 2, css: "uk-card-footer uk-flex-center");
                        h.T("</form>");
                    }
                }
            }, false, 2);
        }

        [Ui("备货", group: 1), Tool(Anchor)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(itemname), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status =").T(OrdAccepted).T(" AND shopid = @2 GROUP BY itemid");
                    dc.Query(p => p.Set(hubid).Set(orgid));
                    while (dc.Next())
                    {
                        h.T("<form class=\"uk-card uk-card-default\">");
                        h.MAIN_("uk-card-body");
                        dc.Let(out short itemid).Let(out string itemname).Let(out short qty).Let(out string unit);
                        h.T(itemname).T("：").T(qty).SP().T(unit);
                        h._MAIN();
                        h.TOOLS(group: 4, css: "uk-card-footer uk-flex-center");
                        h.T("</form>");
                    }
                }
            }, false, 2);
        }

        [Ui("中转", group: 1), Tool(Anchor)]
        public void stocked(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(itemname), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status BETWEEN ").T(OrdStocked).T(" AND ").T(OrdReceived).T(" AND shopid = @2 GROUP BY itemid");
                    dc.Query(p => p.Set(hubid).Set(orgid));
                    while (dc.Next())
                    {
                        h.T("<form class=\"uk-card uk-card-default\">");
                        h.MAIN_("uk-card-body");
                        dc.Let(out short itemid).Let(out string itemname).Let(out short qty).Let(out string unit);
                        h.T(itemname).T("：").T(qty).SP().T(unit);
                        h._MAIN();
                        h.TOOLS(group: 2, css: "uk-card-footer uk-flex-center");
                        h.T("</form>");
                    }
                }
            }, false, 2);
        }

        [Ui("后段", group: 1), Tool(Anchor)]
        public void later(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(itemname), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status BETWEEN ").T(OrdStocked).T(" AND ").T(OrdReceived).T(" AND shopid = @2 GROUP BY itemid");
                    dc.Query(p => p.Set(hubid).Set(orgid));
                    while (dc.Next())
                    {
                        h.T("<form class=\"uk-card uk-card-default\">");
                        h.MAIN_("uk-card-body");
                        dc.Let(out short itemid).Let(out string itemname).Let(out short qty).Let(out string unit);
                        h.T(itemname).T("：").T(qty).SP().T(unit);
                        h._MAIN();
                        h.TOOLS(group: 2, css: "uk-card-footer uk-flex-center");
                        h.T("</form>");
                    }
                }
            }, false, 2);
        }

        [Ui("备货", tip: "为订单池中的订单供货", group: 2), Tool(ButtonShow)]
        public async Task give(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-2];
            bool range = true;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, array_agg(id), array_agg(qty) FROM orders WHERE hubid = @1 AND status =").T(OrdPaid).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @2) GROUP BY itemid");
                    dc.Query(p => p.Set(hubid).Set(orgid));
                }
                wc.GivePage(200, h =>
                {
                    h.FORM_().FIELDUL_("选择供货数量");
                    h.T("<input type=\"range\" class=\"uk-width-1-1\">");
                    h._FIELDUL()._FORM();
                });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                int maxid = f[nameof(maxid)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET giver = @1, given = localtimestamp(), status = ").T(OrdAccepted).T("@1 WHERE id <= @3 AND hubid = @1 AND status = ").T(OrdPaid);
                    dc.Execute(p => p.Set(maxid));
                }
                wc.GiveRedirect();
            }
        }

        [Ui("取消备货", tip: "为订单池中的订单供货", group: 4), Tool(ButtonShow)]
        public async Task ungive(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            bool range = true;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, array_agg(id), array_agg(qty) FROM orders WHERE hubid = @1 AND status =").T(OrdPaid).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @2) GROUP BY itemid");
                    dc.Query(p => p.Set(hubid).Set(orgid));
                }
                wc.GivePage(200, h =>
                {
                    h.FORM_().FIELDUL_("选择供货数量");
                    h.T("<input type=\"range\" class=\"uk-width-1-1\">");
                    h._FIELDUL()._FORM();
                });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                int maxid = f[nameof(maxid)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET giver = @1, given = localtimestamp(), status = ").T(OrdAccepted).T("@1 WHERE id <= @3 AND hubid = @1 AND status = ").T(OrdPaid);
                    dc.Execute(p => p.Set(maxid));
                }
                wc.GiveRedirect();
            }
        }
    }

    [Ui("订单")]
    public class TeamlyOrderWork : OrderWork
    {
        public TeamlyOrderWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<TeamOrderVarWork>();
        }

        [Ui("前段"), Tool(Anchor)]
        public void not(WebContext wc)
        {
            short teamid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status BETWEEN ").T(OrdPaid).T(" AND ").T(OrdAccepted).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD()
                    );
                }
            });
        }

        [Ui("中转"), Tool(Anchor)]
        public void taken(WebContext wc)
        {
            short orgid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdStocked).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(orgid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD()
                    );
                }
            });
        }

        [Ui("派运"), Tool(Anchor)]
        public void sent(WebContext wc)
        {
            short orgid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE status = ").T(OrdSent).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(orgid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Statuses[o.status])
                    );
                }
            });
        }

        [Ui("运达"), Tool(Anchor)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE hubid = @1 AND status = ").T(OrdReceived).T(" AND teamid = @2 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(hubid).Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.utel, o.uname).TD(o.itemname).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD()
                    );
                }
            });
        }

        [Ui(icon: "search", tip: "查找"), Tool(AnchorPrompt)]
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