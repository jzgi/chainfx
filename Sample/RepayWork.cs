using System;
using System.Data;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class RepayWork : Work
    {
        protected RepayWork(WorkConfig cfg) : base(cfg)
        {
        }
    }


    [UserAuthorize(hubly: 1)]
    [Ui("结款")]
    public class HublyRepayWork : RepayWork
    {
        public HublyRepayWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<HublyRepayVarWork>();
        }

        void PutRepays(WebContext wc, Repay[] arr)
        {
            wc.GivePage(200, h =>
            {
                var shops = Obtain<Map<short, Shop>>();
                var teams = Obtain<Map<short, Team>>();

                h.TOOLBAR();
                h.TABLE(arr, null,
                    o =>
                    {
                        h.TD_();
                        if (o.shopid > 0)
                        {
                            h.T(shops[o.shopid]?.name).BR();
                        }
                        else if (o.teamid > 0)
                        {
                            h.T(teams[o.teamid]?.name).BR();
                        }
                        h.T(o.user)._TD();
                        h.TD_().T(o.till)._TD().TD(o.orders).TD(o.cash).TD(o.payer);
                    });
            });
        }

        [Ui("已结"), Tool(Anchor)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM repays WHERE hubid = @1 AND status BETWEEN ").T(Repay.CREATED).T(" AND ").T(Repay.FAILED).T(" ORDER BY status");
                var arr = dc.Query<Repay>(p => p.Set(hubid));
                PutRepays(wc, arr);
            }
        }

        [Ui("已转"), Tool(Anchor)]
        public void paid(WebContext wc)
        {
            string hubid = wc[0];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM repays WHERE hubid = @1 AND status = ").T(Repay.PAID).T(" ORDER BY id DESC");
                var arr = dc.Query<Repay>(p => p.Set(hubid));
                PutRepays(wc, arr);
            }
        }

        // TODO
        [Ui("未结"), Tool(Anchor)]
        public void not(WebContext wc)
        {
            string hubid = wc[0];
            short shopid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT hubid, count(*) AS orders, sum(cash) AS cash FROM orders WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" AND shopid = @2 GROUP BY shopid");
                dc.Query(p => p.Set(hubid).Set(shopid));
                wc.GivePage(200, h => { h.TOOLBAR(); });
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("结算"), Tool(ButtonShow)]
        public async Task calc(WebContext wc)
        {
            string hubid = wc[0];
            DateTime till; // till/before date
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    wc.GivePane(200, h =>
                    {
                        h.FORM_().FIELDUL_("选择截至日期（不包含）");
                        h.LI_().DATE("截　至", nameof(till), DateTime.Today.AddDays(2), max: DateTime.Today)._LI();
                        h._FIELDUL()._FORM();
                    });
                }
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                till = f[nameof(till)];
                using (var dc = NewDbContext(IsolationLevel.ReadUncommitted))
                {
                    // generate for shops
                    dc.Sql("INSERT INTO repays (hubid, shopid, till, orders, total SELECT @1, shopid, @2, count(*) AS orders, sum(shopp) AS total FROM orders WHERE hubid = @hubid AND status = ").T(Order.RECEIVED).T(" AND received < @2 GROUP BY shopid");
                    dc.Execute(p => p.Set(hubid).Set(till));
                    // generate for teams
                    dc.Sql("INSERT INTO repays (hubid, teamid, till, orders, total SELECT @1, teamid, @2, count(*) AS orders, sum(teamp) AS total FROM orders WHERE hubid = @hubid AND status = ").T(Order.RECEIVED).T(" AND received < @2 GROUP BY teamid");
                    dc.Execute(p => p.Set(hubid).Set(till));
                    // generate for senders
                    dc.Sql("INSERT INTO repays (hubid, userid, user, till, orders, total SELECT @1, senderid, last(sender), @2, count(*) AS orders, sum(senderp) AS total FROM orders WHERE hubid = @hubid AND status = ").T(Order.RECEIVED).T(" AND received < @2 GROUP BY senderid");
                    dc.Execute(p => p.Set(hubid).Set(till));

                    // update order status for all related
                    dc.Sql("UPDATE orders SET status = ").T(Order.ENDED).T(" WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" AND received < @2");
                    dc.Execute(p => p.Set(hubid).Set(till));
                }
                wc.GivePane(200);
            }
        }

        struct Tran
        {
            internal int id;
            internal string orgid;
            internal string mgrwx;
            internal string mgrname;
            internal decimal cash;
        }

        [Ui("转款"), Tool(ButtonConfirm)]
        public async Task pay(WebContext wc)
        {
            ValueList<Tran> trans = new ValueList<Tran>(16);
            using (var dc = NewDbContext())
            {
                // retrieve
                if (dc.Query("SELECT r.id, r.orgid, mgrwx, mgrname, cash FROM repays AS r, orgs AS o WHERE r.orgid = o.id AND r.status = 0"))
                {
                    while (dc.Next())
                    {
                        Tran tr;
                        dc.Let(out tr.id).Let(out tr.orgid).Let(out tr.mgrwx).Let(out tr.mgrname).Let(out tr.cash);
                        trans.Add(tr);
                    }
                }
            }
            // do transfer for each
            User prin = (User) wc.Principal;
            for (int i = 0; i < trans.Count; i++)
            {
                var tr = trans[i];
//                string err = await ((SampService) Service).Hub.PostTransferAsync(tr.id, tr.mgrwx, tr.mgrname, tr.cash, "订单结款");
                // update data records
                using (var dc = NewDbContext())
                {
//                    if (err != null) // error occured
//                    {
//                        dc.Execute("UPDATE repays SET err = @1 WHERE id = @2", p => p.Set(err).Set(tr.id));
//                    }
//                    else
//                    {
//                        // update repay status
//                        dc.Execute("UPDATE repays SET err = NULL, payer = @1, status = 1 WHERE id = @2", p => p.Set(prin.name).Set(tr.id));
//                    }
                }
            }
            wc.GiveRedirect();
        }

        [Ui("手动", "强制设为已转款"), Tool(ButtonPickConfirm)]
        public void over(WebContext wc)
        {
        }
    }


    [Ui("结款")]
    public class ShoplyRepayWork : RepayWork
    {
        public ShoplyRepayWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<ShoplyRepayVarWork>();
        }

        [Ui("已结"), Tool(Anchor)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short shopid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM repays WHERE hubid = @1 AND status >= ").T(Repay.CREATED).T(" AND shopid = @2 ORDER BY status");
                var arr = dc.Query<Repay>(p => p.Set(hubid).Set(shopid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null,
                        o => h.TD(o.user).TD_().T(o.till)._TD().TD(o.orders).TD(o.cash).TD(o.payer)
                    );
                });
            }
        }

        [Ui("未结"), Tool(Anchor)]
        public void not(WebContext wc)
        {
            string hubid = wc[0];
            short shopid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT count(*) AS orders, sum(cash) AS cash FROM orders WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" AND shopid = @2 GROUP BY shopid");
                dc.Query(p => p.Set(hubid).Set(shopid));
                wc.GivePage(200, h => { h.TOOLBAR(); });
            }
        }
    }


    [Ui("结款")]
    public class TeamlyRepayWork : RepayWork
    {
        public TeamlyRepayWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<TeamlyRepayVarWork>();
        }

        [Ui("已结"), Tool(Anchor)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM repays WHERE hubid = @1 AND status >= ").T(Repay.CREATED).T(" AND teamid = @2 ORDER BY status");
                var arr = dc.Query<Repay>(p => p.Set(hubid).Set(teamid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null,
                        o => h.TD(o.user).TD_().T(o.till)._TD().TD(o.orders).TD(o.cash).TD(o.payer)
                    );
                });
            }
        }

        [Ui("未结"), Tool(Anchor)]
        public void not(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT count(*) AS orders, sum(cash) AS cash FROM orders WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" AND teamid = @2 GROUP BY teamid");
                dc.Query(p => p.Set(hubid).Set(teamid));
                wc.GivePage(200, h => { h.TOOLBAR(); });
            }
        }
    }
}