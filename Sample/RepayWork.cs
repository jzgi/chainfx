using System;
using System.Data;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Sample
{
    public abstract class RepayWork<V> : Work where V : RepayVarWork
    {
        protected RepayWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, int>();
        }
    }


    [Ui("结款")]
    public class AdmRepayWork : RepayWork<AdmRepayVarWork>
    {
        public AdmRepayWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("新结"), Tool(Anchor)]
        public void @default(WebContext ac, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Query("SELECT * FROM repays WHERE status = 0 ORDER BY id DESC LIMIT 20 OFFSET @1", p => p.Set(page * 20));
                ac.GiveSheetPage(200, dc.ToArray<Repay>(),
                    h => h.TH("网点").TH("起始").TH("截至").TH("总数").TH("单额").TH("净额").TH("转款人"), (h, o) => h.TD(o.shopid).TD(o.fro).TD(o.till).TD(o.orders).TD(o.total).TD(o.cash).TD(o.payer)
                );
            }
        }

        [Ui("已转"), Tool(Anchor)]
        public void old(WebContext ac, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Query("SELECT * FROM repays WHERE status = 1 ORDER BY id DESC LIMIT 20 OFFSET @1", p => p.Set(page * 20));
                ac.GiveSheetPage(200, dc.ToArray<Repay>(),
                    h => h.TH("网点").TH("起始").TH("截至").TH("单数").TH("总额").TH("净额").TH("转款人"), (h, o) => h.TD(o.shopid).TD(o.fro).TD(o.till).TD(o.orders).TD(o.total).TD(o.cash).TD(o.payer)
                );
            }
        }

        [Ui("结算", "生成各网点结款单"), Tool(ButtonShow)]
        public async Task reckon(WebContext ac)
        {
            DateTime fro; // from date
            DateTime till; // till/before date
            if (ac.GET)
            {
                using (var dc = NewDbContext())
                {
                    fro = (DateTime) dc.Scalar("SELECT till FROM repays ORDER BY id DESC LIMIT 1");
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        m.DATE(nameof(fro), fro, "起始", @readonly: true);
                        m.DATE(nameof(till), DateTime.Today, "截至", max: DateTime.Today);
                        m._FORM();
                    });
                }
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                fro = f[nameof(fro)];
                till = f[nameof(till)];
                using (var dc = NewDbContext(IsolationLevel.ReadUncommitted))
                {
                    dc.Execute(@"INSERT INTO repays (shopid, fro, till, orders, total, cash) 
                    SELECT shopid, @1, @2, COUNT(*), SUM(total), SUM(total * 0.994) FROM orders WHERE status = " + Order.FINISHED + " AND finished >= @1 AND finished < @2 GROUP BY shopid", p => p.Set(fro).Set(till));
                }
                ac.GivePane(200);
            }
        }

        struct Tran
        {
            internal int id;
            internal string shopid;
            internal string mgrwx;
            internal string mgrname;
            internal decimal cash;
        }

        [Ui("转款", "按结款单转款给网点"), Tool(ButtonConfirm)]
        public async Task pay(WebContext ac)
        {
            Roll<Tran> trans = new Roll<Tran>(16);
            using (var dc = NewDbContext())
            {
                // retrieve
                if (dc.Query("SELECT r.id, r.shopid, mgrwx, mgrname, cash FROM repays AS r, shops AS s WHERE r.shopid = s.id AND r.status = 0"))
                {
                    while (dc.Next())
                    {
                        Tran tr;
                        dc.Let(out tr.id).Let(out tr.shopid).Let(out tr.mgrwx).Let(out tr.mgrname).Let(out tr.cash);
                        trans.Add(tr);
                    }
                }
            }
            // do transfer for each
            User prin = (User) ac.Principal;
            for (int i = 0; i < trans.Count; i++)
            {
                var tr = trans[i];
                string err = await WeiXinUtility.PostTransferAsync(tr.id, tr.mgrwx, tr.mgrname, tr.cash, "订单结款");
                // update data records
                using (var dc = NewDbContext())
                {
                    if (err != null) // error occured
                    {
                        dc.Execute("UPDATE repays SET err = @1 WHERE id = @2", p => p.Set(err).Set(tr.id));
                    }
                    else
                    {
                        // update repay status
                        dc.Execute("UPDATE repays SET err = NULL, payer = @1, status = 1 WHERE id = @2", p => p.Set(prin.name).Set(tr.id));
                        // add a cash journal entry
                        var ety = new Cash
                        {
                            shopid = tr.shopid,
                            date = DateTime.Now,
                            txn = 11, // sales income
                            descr = "订单结款",
                            received = tr.cash,
                            paid = 0,
                            keeper = prin.name
                        };
                        const byte proj = 0xff ^ Cash.ID;
                        dc.Execute(dc.Sql("INSERT INTO cashes")._(ety, proj)._VALUES_(ety, proj), p => ety.Write(p, proj));
                    }
                }
            }
            ac.GiveRedirect();
        }
    }
}