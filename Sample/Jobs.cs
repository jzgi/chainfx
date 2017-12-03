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
            CreateVar<OprVarWork, string>(prin => ((User) prin).oprat, prin => ((User) prin).oprname);
        }
    }

    [Ui("常规"), Allow(adm: true)]
    public class AdmWork : Work
    {
        public AdmWork(WorkContext wc) : base(wc)
        {
            Create<AdmOprWork>("opr");

            Create<AdmShopWork>("shop");

            Create<AdmRepayWork>("repay");

            Create<AdmKickWork>("kick");
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
                        h.CAPTION(false, "系统运行状况", "运行中", true);
                        h.FIELDSET_("参数");
                        h.FIELD("2.0", "版本");
                        h._FIELDSET();
                    });
                });
            }
            else
            {
                ac.GiveFrame(200, false, 60 * 15, "粗粮达人平台管理");
            }
        }

        [Ui("清除"), UiTool(UiMode.ButtonOpen, 2)]
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