using System.Threading.Tasks;
using SkyCloud.Web;

namespace SkyCloud.Chain
{
    [Ui("Peers")]
    public class PeerWork : WebWork
    {
        public void @default(WebContext wc)
        {
            using var dc = NewDbContext();
            var arr = dc.Query<Peer>("SELECT * FROM chain.peers WHERE id != '&'");
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.TABLE(arr, o =>
                {
                    h.TDCHECK(o.id);
                    h.TD(o.id);
                    h.TD(o.name);
                    h.TD(o.raddr);
                    h.TD(Login.Statuses[o.status]);
                    h.TDFORM(() => h.VARTOOLS(o.id));
                });
            });
        }

        [Ui("➕", "Create A New Peer"), Tool(Modal.ButtonShow, Size.Large)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Peer { };
                wc.GivePane(200, h =>
                {
                    const string css = "uk-width-small";
                    h.FORM_().FIELDSUL_("Attributes");
                    h.LI_().LABEL("ID", css).TEXT(null, nameof(o.id), o.id, max: 10, required: true)._LI();
                    h.LI_().LABEL("Name", css).TEXT(null, nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().LABEL("Address", css).URL(null, nameof(o.raddr), o.raddr, max: 20, required: true)._LI();
                    h.LI_().LABEL("Status", css).SELECT(null, nameof(o.status), o.status, Login.Statuses, required: true)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync<Peer>();
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.peers ").colset(o)._VALUES_(o);
                await dc.ExecuteAsync(p => o.Write(p));
                wc.GivePane(200); // close dialog
            }
        }
    }
}