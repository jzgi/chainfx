using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class RepayWork<V> : Work where V : RepayVarWork
    {
        protected RepayWork(WorkContext wc) : base(wc)
        {
            CreateVar<V>();
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
                ac.GiveFormPane(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }
    }

    [Ui("结款")]
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
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Repay>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Repay>) null);
                }
            }
        }

        [Ui("生成结款单")]
        public void @new(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.GiveFormPane(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        [Ui("结款")]
        public async Task pay(ActionContext ac)
        {
            string shopid = ac[Parent];

            int id = ac.Query[nameof(id)];


            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;

                await WeiXinUtility.PostTransferAsync();

                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.GiveFormPane(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        [Ui("余额管理")]
        public void balance(ActionContext ac)
        {
        }
    }

    [Ui("结款管理")]
    public class DvrRepayWork : RepayWork<DvrRepayVarWork>
    {
        public DvrRepayWork(WorkContext wc) : base(wc)
        {
        }
        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1 AND status < 4", p => p.Set(shopid)))
                {
                    ac.GiveGridFormPage(200, dc.ToList<Repay>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Repay>) null);
                }
            }
        }

    }

    [Ui("结款管理")]
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
                    ac.GiveGridFormPage(200, dc.ToList<Repay>());
                }
                else
                {
                    ac.GiveGridFormPage(200, (List<Repay>) null);
                }
            }
        }

    }
}