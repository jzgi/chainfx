using Greatbone;

namespace Samp
{
    [UserAuthorize(teamly: 1)]
    [Ui("动态")]
    public class TeamlyVarWork : Work, IOrgVar
    {
        public TeamlyVarWork(WorkConfig cfg) : base(cfg)
        {
            Make<TeamlyOrderWork>("order");

            Make<TeamlyUserWork>("user");

            Make<TeamlyOprWork>("opr");

            Make<TeamlyRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short id = wc[this];
            var org = Obtain<Map<short, Team>>()[id];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 900, org?.name);
            }
            else
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();

                    h.SECTION_("uk-card uk-card-default");
                    h.HEADER_("uk-card-header").H4("订单")._HEADER();
                    h.MAIN_("uk-card-body")._MAIN();
                    h._SECTION();

                    h.SECTION_("uk-card uk-card-default");
                    h.HEADER_("uk-card-header").H4("成员")._HEADER();
                    h.MAIN_("uk-card-body")._MAIN();
                    h._SECTION();
                });
            }
        }
    }
}