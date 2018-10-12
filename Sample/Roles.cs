using Greatbone;

namespace Samp
{
    public class MyWork : Work
    {
        public MyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<MyVarWork>((obj) => ((User) obj).id);
        }
    }

    [UserAccess(hubly: 1)]
    [Ui("动态")]
    public class HublyWork : Work
    {
        public HublyWork(WorkConfig cfg) : base(cfg)
        {
            Make<HubOrderWork>("order");

            Make<HubItemWork>("item");

            Make<HublyTeamWork>("org");

            Make<HublyShopWork>("shop");

            Make<HubUserWork>("opr");

            Make<HubRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
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
                var hub = Obtain<Map<string, Hub>>()[hubid];
                wc.GiveFrame(200, false, 900, hub.name);
            }
        }
    }

    public class ShoplyWork : Work
    {
        public ShoplyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<ShoplyVarWork>(prin => ((User) prin).shopid);
        }
    }


    public class OrglyWork : Work
    {
        public OrglyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<TeamlyVarWork>(prin => ((User) prin).teamid);
        }
    }
}