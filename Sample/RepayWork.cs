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
    [User(User.AID)]
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

        [Ui("发起清算", "为商家清算已完成的订单并生成结款单", Mode = UiMode.ButtonShow)]
        public async Task reckon(ActionContext ac)
        {
            DateTime now = DateTime.Now;
            int term = 0;
            DateTime till = now;

            DateTime end = default(DateTime);
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var ret = dc.Scalar("SELECT created FROM repays ORDER BY id DESC LIMIT 1");
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        if (ret != null)
                        {
                            m.CALLOUT("上次清算时间是" + (DateTime) ret, false);
                        }
                        m.DATE(nameof(end), end, "截止日期");
                        m._FORM();
                    });
                }
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                end = f[nameof(end)];
                using (var dc = ac.NewDbContext())
                {
                    // compute
                    int ret = (int) dc.Scalar("SELECT reckon()");

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