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

    [UserAuthorize(hubly: 1)]
    [Ui("动态")]
    public class HublyWork : Work
    {
        public HublyWork(WorkConfig cfg) : base(cfg)
        {
            Make<HublyOrderWork>("order");

            Make<HublyItemWork>("item");

            Make<HublyShopWork>("shop");

            Make<HublyTeamWork>("team");

            Make<HublyRepayWork>("repay");

            Make<HublyOprWork>("opr");
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
                        h.HEADER_("uk-card-header").H4("订单")._HEADER();
                        h.MAIN_("uk-card-body")._MAIN();
                        h._SECTION();

                        h.SECTION_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").H4("货架")._HEADER();
                        h.MAIN_("uk-card-body")._MAIN();
                        h._SECTION();

                        h.SECTION_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").H4("工坊")._HEADER();
                        h.MAIN_("uk-card-body")._MAIN();
                        h._SECTION();

                        h.SECTION_("uk-card uk-card-default");
                        h.HEADER_("uk-card-header").H4("团组")._HEADER();
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


    public class TeamlyWork : Work
    {
        public TeamlyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<TeamlyVarWork>(prin => ((User) prin).teamid);
        }
    }
}