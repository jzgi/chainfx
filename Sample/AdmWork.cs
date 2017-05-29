using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("维护")]
    [User(adm: true)]
    public class AdmWork : Work
    {
        public AdmWork(WorkContext wc) : base(wc)
        {
            Create<AdmUserWork>("user"); // users management

            Create<AdmRepayWork>("repay"); // repays present

            Create<AdmPastRepayWork>("pastrepay"); // repays past

            Create<AdmFeekbackWork>("feedback");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }

        [Ui("订单存档")]
        public void archive(ActionContext ac)
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
}