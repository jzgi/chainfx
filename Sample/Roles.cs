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

    [UserAuth(reg: 1)]
    [Ui("首页")]
    public class RegWork : Work
    {
        public RegWork(WorkConfig cfg) : base(cfg)
        {
            Create<RegOrderWork>("order");

            Create<RegItemWork>("item");

            Create<RegChatWork>("chat");

            Create<RegUserWork>("user");

            Create<RegOrgWork>("org");

            Create<RegRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                using (var dc = NewDbContext())
                {
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR();
                        h.SECTION_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").H4("网点")._HEADER();
                        h.MAIN_("uk-card-body")._MAIN();
                        h._SECTION();

                        h.SECTION_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").H4("订单")._HEADER();
                        h.MAIN_("uk-card-body")._MAIN();
                        h._SECTION();
                    });
                }
            }
            else
            {
                wc.GiveFrame(200, false, 900, "调度服务");
            }
        }
    }

    public class ShopWork : Work
    {
        public ShopWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<OprVarWork, string>(prin => ((User) prin).shopat);
        }
    }


    public class TeamWork : Work
    {
        public TeamWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<TeamVarWork, string>(prin => ((User) prin).teamat);
        }
    }
}