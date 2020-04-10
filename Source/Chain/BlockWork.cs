using SkyCloud.Web;

namespace SkyCloud.Chain
{
    [Ui("Blocks")]
    public class BlockWork : WebWork
    {
        [Ui("All"), Tool(Modal.Anchor)]
        public void @default(WebContext wc)
        {
            using var dc = NewDbContext();
            
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.FORM_("uk-card uk-card-primary");
                h._UL();
                h._FORM();
            });
        }

        [Ui("Local"), Tool(Modal.Anchor)]
        public void local(WebContext wc)
        {
        }

        [Ui("&#128269;"), Tool(Modal.AnchorPrompt)]
        public void search(WebContext wc)
        {
        }
    }
}