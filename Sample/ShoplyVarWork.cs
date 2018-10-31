using Greatbone;

namespace Samp
{
    [UserAuthorize(shoply: 1)]
    [Ui("动态")]
    public class ShoplyVarWork : Work, IOrgVar
    {
        public ShoplyVarWork(WorkConfig cfg) : base(cfg)
        {
            Make<ShoplyOrderWork>("order");

            Make<ShoplyOprWork>("user");

            Make<ShoplyRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short id = wc[this];
            var shop = Obtain<Map<short, Team>>()[id];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 900, title: shop?.name);
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
                    h.HEADER_("uk-card-header").H4("人员")._HEADER();
                    h.MAIN_("uk-card-body")._MAIN();
                    h._SECTION();
                });
            }
        }
    }
}