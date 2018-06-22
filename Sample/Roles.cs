using Greatbone;

namespace Samp
{
    public class MyWork : Work
    {
        public MyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyVarWork, int>((obj) => ((User) obj).id);
        }
    }

    public class OprWork : Work
    {
        public OprWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<OprVarWork, string>(prin => ((User) prin).oprat);
        }
    }

    [UserAccess(adm: 1)]
    [Ui("常规")]
    public class AdmWork : Work
    {
        public AdmWork(WorkConfig cfg) : base(cfg)
        {
            Create<AdmOprWork>("opr");

            Create<AdmOrgWork>("org");

            Create<AdmRepayWork>("repay");

            Create<AdmInfWork>("inf");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.T("<article class=\"uk-card uk-card-primary uk-card-body\">");
                    h.T("<h4>系统信息</h4>");
                    h.P("2.0", "版　本");
                    h.T("</article>");
                });
            }
            else
            {
                wc.GiveFrame(200, false, 60 * 15, "平台管理");
            }
        }

        [Ui("清理"), Tool(Modal.ButtonOpen)]
        public void clean(WebContext ac)
        {
        }
    }
}