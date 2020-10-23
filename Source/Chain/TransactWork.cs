using System.Threading.Tasks;
using SkyChain.Web;

namespace SkyChain.Chain
{
    [Ui("事务")]
    public class TransactWork : WebWork
    {
        public void @default(WebContext wc)
        {
            using var dc = NewDbContext();
            var arr = dc.Query<Operation>("SELECT * FROM chain.logins");
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.TABLE(arr, o =>
                {
                    h.TDCHECK(o.acct);
                    h.TD(o.acct);
                    h.TD(o.step);
                    h.TDFORM(() => h.VARTOOLS(o.acct));
                });
            });
        }

        [Ui("➕", "Create A New Login"), Tool(Modal.ButtonShow, Size.Large)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Operation { };
                wc.GivePane(200, h =>
                {
                    const string css = "uk-width-small";
                    h.FORM_().FIELDSUL_("Login Attributes");
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
               
                wc.GivePane(200); // close dialog
            }
        }
    }
}