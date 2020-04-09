using SkyCloud.Web;

namespace SkyCloud.Net
{
    [Ui("Peers")]
    public class PeerWork : WebWork
    {
        public void @default(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.FORM_("uk-card uk-card-primary");
                h._UL();
                h._FORM();
            });
        }

        [Ui("New", "Create New Peer"), Tool(Modal.ButtonShow)]
        public void @new(WebContext wc)
        {
        }
    }
}