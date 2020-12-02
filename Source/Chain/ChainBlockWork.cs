using SkyChain.Web;

namespace SkyChain.Chain
{
    [Ui("块链")]
    public class ChainBlockWork : WebWork
    {
        [Ui("本地"), Tool(Modal.Anchor)]
        public void @default(WebContext wc)
        {
            using var dc = NewDbContext();

            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 1);
                h.FORM_("uk-card uk-card-primary");
                h._UL();
                h._FORM();
            });
        }

        [Ui("全链"), Tool(Modal.Anchor)]
        public void global(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(group: 2);
                h.FORM_("uk-card uk-card-primary");
                h._UL();
                h._FORM();
            });
        }

        [Ui("查询"), Tool(Modal.AnchorPrompt)]
        public void search(WebContext wc)
        {
        }
    }
}