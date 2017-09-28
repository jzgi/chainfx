using Greatbone.Core;

namespace Greatbone.Sample
{
    public class MyWork : Work
    {
        public MyWork(WorkContext wc) : base(wc)
        {
            CreateVar<MyVarWork, string>((obj) => ((User) obj).wx);
        }
    }

    public class OprWork : Work
    {
        public OprWork(WorkContext wc) : base(wc)
        {
            CreateVar<OprVarWork, string>((prin) => ((User) prin).oprat);
        }
    }

    [Ui("常规")]
    [User(adm: true)]
    public class AdmWork : Work
    {
        public AdmWork(WorkContext wc) : base(wc)
        {
            Create<AdmUserWork>("user");

            Create<AdmShopWork>("shop");

            Create<AdmItemWork>("item");

            Create<AdmKickWork>("kick");

            Create<AdmUserWork>("user"); // users management

            Create<AdmRepayWork>("repay"); // repays present

            Create<AdmPastRepayWork>("pastrepay"); // repays past
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
                    ac.GiveGridPage(200, dc.ToArray<Repay>(), (h, o) => { });
                }
                else
                {
                    ac.GiveGridPage(200, (Repay[]) null, null);
                }
            }
        }
    }
}