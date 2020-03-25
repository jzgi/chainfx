using CloudUn.Web;

namespace CloudUn.Net
{
    [Ui("Types")]
    public class TypWork : WebWork
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
    }
}