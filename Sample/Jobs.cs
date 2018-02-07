using Greatbone.Core;

namespace Greatbone.Sample
{
    public class MyWork : Work
    {
        public MyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyVarWork, string>((obj) => ((User) obj).wx);
        }
    }

    public class OprWork : Work
    {
        public OprWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<OprVarWork, string>(prin => ((User) prin).oprat);
        }
    }

    [Ui("常规"), User(adm: true)]
    public class AdmWork : Work
    {
        public AdmWork(WorkConfig cfg) : base(cfg)
        {
            Create<AdmOprWork>("opr");

            Create<AdmShopWork>("shop");

            Create<AdmRepayWork>("repay");

            Create<AdmKickWork>("kick");

            Create<AdmSlideWork>("lesson");
        }

        public void @default(ActionContext ac)
        {
            bool inner = ac.Query[nameof(inner)];
            if (inner)
            {
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.BOARDVIEW(h =>
                    {
                        h.CAPTION("系统运行状况", "运行中", true);
                        h.FIELD("2.0", "版本");
                        h.TAIL();
                    });
                });
            }
            else
            {
                ac.GiveFrame(200, false, 60 * 15, "粗粮达人平台管理");
            }
        }

        [Ui("清理"), Tool(Modal.ButtonOpen, 2)]
        public void clean(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM repays WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.GiveBoardPage(200, dc.ToArray<Repay>(), (h, o) => { });
                }
                else
                {
                    ac.GiveBoardPage(200, (Repay[]) null, null);
                }
            }
        }
    }
}