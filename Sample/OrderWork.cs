using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

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
                            h.T("收货：").T(o.custaddr).SP().T(o.cust).SP().T(o.custtel);
                            h._HEADER();
                            h.MAIN_("uk-card-body uk-row");
                            h.PIC_(css: "uk-width-1-6").T("/").T(hubid).T("/").T(o.itemid).T("/icon")._PIC();
                            h.DIV_("uk-width-2-3").SP().T(o.item).SP().CNY(o.price).T(o.qty).T("/").T(o.unit)._DIV();
                            h.VARTOOLS(css: "uk-width-1-6");
                            h._MAIN();
                        }
                    );
                }, false, 3, title: "我的订单", refresh: 120);
            }
        }
    }

    [Ui("订单")]
    public class HublyOrderWork : OrderWork
    {
        public HublyOrderWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<HubOrderVarWork>();
        }

        static void PutRoll(HtmlContent h, OrderRoll o, IOrg org, byte vargrp)
        {
            h.HEADER_("uk-card-header");
            h.T(org is Team ? "&#11093;" : "&#9995;").T(org.Name);
            h.DIV_(css: "uk-badge").T(o.Oprs)._DIV();
            h._HEADER();
            h.MAIN_("uk-card-body uk-flex");
            h.UL_(css: "uk-child-width-1-2 uk-grid1");
            for (int i = 0; i < o.Count; i++)
            {
                var v = o[i];
                h.LI_().T(v.item).SP().T(v.qty)._LI();
            }
            h._UL();
            h._MAIN();
            h.VARTOOLS(group: vargrp, css: "uk-card-footer uk-flex-between");
        }

        [Ui("排队", group: 1), Tool(Anchor)]
        public void @default(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 0b000011);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT teamid AS no, itemid, first(item) AS item, sum(qty) AS qty, sum(cash) AS cash FROM orders WHERE hubid = @1 AND status = ").T(Order.PAID).T(" GROUP BY teamid, itemid");
                    var arr = dc.Query<OrderAgg>(p => p.Set(hubid));
                    var rolls = arr.RollUp<OrderRoll, short, OrderAgg>(x => x.no);
                    h.BOARD(rolls, o =>
                        {
                            var team = Obtain<Map<short, Team>>()[o.Key];
                            PutRoll(h, o, team, 0b000011);
                        }
                    );
                }
            }, false, 3, refresh: 720);
        }

        [Ui("备货", group: 1), Tool(Anchor)]
        public void accepted(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 0b000101);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT shopid AS no, itemid, first(item) AS item, sum(qty) AS qty, sum(cash) AS cash, array_agg(DISTINCT accepter) AS oprs FROM orders WHERE hubid = @1 AND status = ").T(Order.CONFIRMED).T(" GROUP BY shopid, itemid");
                    var arr = dc.Query<OrderAgg>(p => p.Set(hubid));
                    var rolls = arr.RollUp<OrderRoll, short, OrderAgg>(x => x.no);
                    h.BOARD(rolls, o =>
                        {
                            var shop = Obtain<Map<short, Shop>>()[o.Key];
                            PutRoll(h, o, shop, 0b000101);
                        }
                    );
                }
            }, false, 3, refresh: 720);
        }

        [Ui("中转", group: 1), Tool(Anchor)]
        public void stocked(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 0b001001);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT teamid AS no, itemid, first(item) AS item, sum(qty) AS qty, sum(cash) AS cash, array_agg(DISTINCT stocker) AS oprs FROM orders WHERE hubid = @1 AND status = ").T(Order.ACCEPTED).T(" GROUP BY teamid, itemid");
                    var arr = dc.Query<OrderAgg>(p => p.Set(hubid));
                    var rolls = arr.RollUp<OrderRoll, short, OrderAgg>(x => x.no);
                    h.BOARD(rolls, o =>
                        {
                            var team = Obtain<Map<short, Team>>()[o.Key];
                            PutRoll(h, o, team, 0b001001);
                        }
                    );
                }
            }, false, 3, refresh: 720);
        }

        [Ui("派运", group: 1), Tool(Anchor)]
        public void sent(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 0b010001);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT teamid AS no, itemid, first(item) AS item, sum(qty) AS qty, sum(cash) AS cash, array_agg(DISTINCT sender) AS oprs FROM orders WHERE hubid = @1 AND status = ").T(Order.SENT).T(" GROUP BY teamid, itemid");
                    var arr = dc.Query<OrderAgg>(p => p.Set(hubid));
                    var rolls = arr.RollUp<OrderRoll, short, OrderAgg>(x => x.no);
                    h.BOARD(rolls, o =>
                        {
                            var team = Obtain<Map<short, Team>>()[o.Key];
                            PutRoll(h, o, team, 0b010001);
                        }
                    );
                }
            }, false, 3, refresh: 720);
        }

        [Ui("运达", group: 1), Tool(Anchor)]
        public void received(WebContext wc, int page)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 0b100001);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT teamid AS no, itemid, first(item) AS item, sum(qty) AS qty, sum(cash) AS cash, array_agg(DISTINCT receiver) AS oprs FROM orders WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" GROUP BY teamid, itemid");
                    var arr = dc.Query<OrderAgg>(p => p.Set(hubid));
                    var rolls = arr.RollUp<OrderRoll, short, OrderAgg>(x => x.no);
                    h.BOARD(rolls, o =>
                        {
                            var team = Obtain<Map<short, Team>>()[o.Key];
                            PutRoll(h, o, team, 0b100001);
                        }
                    );
                }
            }, false, 3, refresh: 720);
        }

        [Ui("&#9052;", "查找", 1), Tool(AnchorPrompt)]
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
                        h.TOOLBAR(group: 1);
                        h.TABLE(arr, null,
                            o => h.TD(o.custtel, o.cust).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Order.Statuses[o.status])
                        );
                    });
                }
            }
        }

        const string Sigma = "&#931;";

        const string Agg = "汇总订单;";


        [Ui(Sigma, Agg, group: 0b000011), Tool(ButtonPickOpen)]
        public void _(WebContext wc)
        {
        }

        [Ui(Sigma, Agg, group: 0b000101), Tool(ButtonPickOpen)]
        public void confirmed_(WebContext wc)
        {
        }

        [Ui(Sigma, Agg, group: 0b001001), Tool(ButtonPickOpen)]
        public void accepted_(WebContext wc)
        {
        }

        [Ui(Sigma, Agg, group: 0b010001), Tool(ButtonPickOpen)]
        public void sent_(WebContext wc)
        {
        }

        [Ui(Sigma, Agg, group: 0b100001), Tool(ButtonPickOpen)]
        public void received_(WebContext wc)
        {
        }
    }

    /// <summary>
    /// The order processing in a workshop.
    /// </summary>
    [Ui("订单")]
    public class ShoplyOrderWork : OrderWork
    {
        public ShoplyOrderWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<ShopOrderVarWork>();
        }

        [Ui("排队", @group: 1), Tool(Anchor)]
        public void not(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(item), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status =").T(Order.PAID).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @2) GROUP BY itemid");
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

        [Ui("备货", @group: 1), Tool(Anchor)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(item), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status =").T(Order.CONFIRMED).T(" AND shopid = @2 GROUP BY itemid");
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

        [Ui("中转", @group: 1), Tool(Anchor)]
        public void stocked(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(item), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status BETWEEN ").T(Order.ACCEPTED).T(" AND ").T(Order.RECEIVED).T(" AND shopid = @2 GROUP BY itemid");
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

        [Ui("后段", @group: 1), Tool(Anchor)]
        public void later(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, last(itemname), sum(qty), last(unit) FROM orders WHERE hubid = @1 AND status BETWEEN ").T(Order.ACCEPTED).T(" AND ").T(Order.RECEIVED).T(" AND shopid = @2 GROUP BY itemid");
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

        [Ui("备货", tip: "为订单池中的订单供货", @group: 2), Tool(ButtonShow)]
        public async Task give(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-2];
            bool range = true;
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, array_agg(id), array_agg(qty) FROM orders WHERE hubid = @1 AND status =").T(Order.PAID).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @2) GROUP BY itemid");
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
                    dc.Sql("UPDATE orders SET giver = @1, given = localtimestamp(), status = ").T(Order.CONFIRMED).T("@1 WHERE id <= @3 AND hubid = @1 AND status = ").T(Order.PAID);
                    dc.Execute(p => p.Set(maxid));
                }
                wc.GiveRedirect();
            }
        }

        [Ui("取消备货", tip: "为订单池中的订单供货", @group: 4), Tool(ButtonShow)]
        public async Task ungive(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[-1];
            bool range = true;
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT itemid, array_agg(id), array_agg(qty) FROM orders WHERE hubid = @1 AND status =").T(Order.PAID).T(" AND itemid IN (SELECT id FROM items WHERE shopid = @2) GROUP BY itemid");
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
                    dc.Sql("UPDATE orders SET giver = @1, given = localtimestamp(), status = ").T(Order.CONFIRMED).T("@1 WHERE id <= @3 AND hubid = @1 AND status = ").T(Order.PAID);
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
        public void pre(WebContext wc)
        {
            short teamid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Order.Empty).T(" FROM orders WHERE status BETWEEN ").T(Order.PAID).T(" AND ").T(Order.CONFIRMED).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.custtel, o.cust).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD()
                    );
                }
            });
        }

        [Ui("中转"), Tool(Anchor)]
        public void accepted(WebContext wc)
        {
            short orgid = wc[Parent];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Order.Empty).T(" FROM orders WHERE status = ").T(Order.ACCEPTED).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(orgid));
                    h.TABLE(arr, null,
                        o => h.TD(o.custtel, o.cust).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD()
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
                    dc.Sql("SELECT ").collst(Order.Empty).T(" FROM orders WHERE status = ").T(Order.SENT).T(" AND teamid = @1 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(orgid));
                    h.TABLE(arr, null,
                        o => h.TD(o.custtel, o.cust).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Order.Statuses[o.status])
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
                    dc.Sql("SELECT ").collst(Order.Empty).T(" FROM orders WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" AND teamid = @2 ORDER BY id");
                    var arr = dc.Query<Order>(p => p.Set(hubid).Set(teamid));
                    h.TABLE(arr, null,
                        o => h.TD(o.custtel, o.cust).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD()
                    );
                }
            });
        }

        [Ui(tip: "查找"), Tool(AnchorPrompt)]
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
                            o => h.TD(o.custtel, o.cust).TD(o.item).TD_(css: "uk-text-right").T(o.qty).SP().T(o.unit)._TD().TD(Order.Statuses[o.status])
                        );
                    });
                }
            }
        }
    }
}