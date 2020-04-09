using SkyCloud.Web;

namespace SkyCloud.Net
{
    [Ui("Users")]
    public class UserWork : WebWork
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

        [Ui("New"), Tool(Modal.ButtonShow)]
        public void @new(WebContext wc)
        {
        }
    }
}