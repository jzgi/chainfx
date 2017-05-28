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
    }

    [Ui("结款")]
    [User(User.MANAGER)]
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
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status = 0", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToDatas<Repay>());
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null);
                }
            }
        }

        [Ui("发起清算", "为商家清算已完成的订单并生成结款单", Mode = UiMode.ButtonShow)]
        public async Task reckon(ActionContext ac)
        {
            DateTime thru; // through date
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var ret = dc.Scalar("SELECT thru FROM repays ORDER BY id DESC LIMIT 1");
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        if (ret != null)
                        {
                            m.CALLOUT("上次清算时间是" + (DateTime) ret, false);
                        }
                        thru = DateTime.Today.AddDays(-1);
                        m.DATE(nameof(thru), thru, "截止日期", max: thru);
                        m._FORM();
                    });
                }
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                thru = f[nameof(thru)];
                using (var dc = ac.NewDbContext())
                {
                    // compute
                    int ret = (int) dc.Scalar("SELECT reckon(@1)", p => p.Set(thru));

                    // view result
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