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

    public class GrplyWork : Work
    {
        public GrplyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<GrplyVarWork, string>(prin => ((User) prin).grpat);
        }
    }

    public class VdrlyWork : Work
    {
        public VdrlyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<GrplyVarWork, string>(prin => ((User) prin).grpat);
        }
    }

    public class CtrlyWork : Work
    {
        public CtrlyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<CtrlyVarWork, string>(prin => ((User) prin).ctrat);
        }
    }

    [UserAccess(adm: 1)]
    [Ui("常规")]
    public class PlatlyWork : Work
    {
        public PlatlyWork(WorkConfig cfg) : base(cfg)
        {
            Create<AdmOprWork>("opr");

            Create<AdmOrgWork>("org");

            Create<AdmRepayWork>("repay");

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