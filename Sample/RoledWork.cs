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
            CreateVar<OprVarWork, short>(prin => ((User) prin).oprat, prin => ((User) prin).oprname);
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

            Create<AdmKickWork>("kick");

            Create<AdmRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200, false, 60 * 15, "平台管理");
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