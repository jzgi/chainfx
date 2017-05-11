using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class RepayWork<V> : Work where V : RepayVarWork
    {
        protected RepayWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, int>();
        }

        public void _cat_(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                // ac.GiveFormPane(303, dc, (i, o) =>
                // {
                //     o.Put(nameof(name), name = i.GetString());
                //     o.Put(nameof(age), age = i.GetInt());
                // }); // see other
            }
        }
    }

    [Ui("结款")]
    [User(User.ASSISTANT)]
    public class OprRepayWork : RepayWork<OprRepayVarWork>
    {
        public OprRepayWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }
    }

    [Ui("当前结款")]
    public class SprRepayWork : RepayWork<SprRepayVarWork>
    {
        public SprRepayWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }

        [Ui("发起")]
        public void calc(ActionContext ac)
        {
        }
    }

    [Ui("以往结款")]
    public class SprPastRepayWork : RepayWork<SprRepayVarWork>
    {
        public SprPastRepayWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }

        [Ui("发起")]
        public void calc(ActionContext ac)
        {
        }
    }

    [Ui("当前结款")]
    public class AdmRepayWork : RepayWork<AdmRepayVarWork>
    {
        public AdmRepayWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }

        [Ui("结款")]
        public void @new(ActionContext ac)
        {
            DateTime now = DateTime.Now;
            int term = 0;
            DateTime till = now;
            using (var dc = ac.NewDbContext())
            {
                dc.Execute(@"INSERT INTO repays (term, shopid, shopname, city, mgrwx, orders, total, amount, till, created)
                    SELECT @1, shopid, COUNT(*), SUM(total), (SUM(total) * 0.994), @2, @3 FROM orders WHERE status = 7 AND closed < @2 GROUP BY shopid
                    ON CONFLICT DO NOTHING",
                    p => p.Set(term).Set(till).Set(now));
                if (dc.Query("SELECT * FROM repays WHERE status = 0"))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }
    }

    [Ui("以往结款")]
    public class AdmPastRepayWork : RepayWork<AdmRepayVarWork>
    {
        public AdmPastRepayWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }
    }
}