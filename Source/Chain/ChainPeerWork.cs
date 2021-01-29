using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Chain
{
    [Ui("Peers")]
    public class ChainPeerWork : WebWork
    {
        public void @default(WebContext wc)
        {
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.BOARD(ChainEnviron.Clients, ety =>
                {
                    var cli = ety.Value;

                    h.HEADER_("uk-card-header");
                    h.T(cli.Info.name);
                    h._HEADER();

                    h.SECTION_("uk-card-body");

                    h._SECTION();
                });
            });
        }

        [Ui("✛ New"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Peer { };
                wc.GivePane(200, h =>
                {
                    const string css = "uk-width-small";
                    h.FORM_().FIELDSUL_("Attributes");
                    h.LI_().LABEL("ID", css).NUMBER(null, nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().LABEL("Name", css).TEXT(null, nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().LABEL("Address", css).URL(null, nameof(o.uri), o.uri, max: 20, required: true)._LI();
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