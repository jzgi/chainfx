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

            Create<AdmOrgWork>("org");

            Create<AdmRepayWork>("repay");
        }

        public void @default(WebContext ac)
        {
            bool inner = ac.Query[nameof(inner)];
            if (inner)
            {
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.BOARDVIEW(h =>
                    {
                        h.CARD_HEADER("系统运行状况", "运行中", true);
                        h.FIELD("2.0", "版本");
                        h.CARD_FOOTER();
                    });
                });
            }
            else
            {
                ac.GiveFrame(200, false, 60 * 15, "粗粮达人平台管理");
            }
        }

        [Ui("清理"), Tool(Modal.ButtonOpen, 2)]
        public void clean(WebContext ac)
        {
        }
    }
}