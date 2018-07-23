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

    public class TmWork : Work
    {
        public TmWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<TmVarWork, string>(prin => ((User) prin).tmat);
        }
    }

    public class VdrWork : Work
    {
        public VdrWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<VdrVarWork, string>(prin => ((User) prin).tmat);
        }
    }

    public class CtrWork : Work
    {
        public CtrWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<CtrVarWork, string>(prin => ((User) prin).ctrat);
        }
    }

    [UserAccess(plat: 1)]
    [Ui("常规")]
    public class PlatWork : Work
    {
        public PlatWork(WorkConfig cfg) : base(cfg)
        {
            Create<PlatUserWork>("user");

            Create<PlatCtrWork>("ctr");

            Create<PlatRepayWork>("repay");
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