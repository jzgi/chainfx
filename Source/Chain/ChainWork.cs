using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Chain
{
    [Ui("&#128160;")]
    public class ChainWork : WebWork
    {
        [Ui("Native"), Tool(Anchor)]
        public virtual async Task @default(WebContext wc)
        {
            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers WHERE native = TRUE");
            var o = await dc.QueryTopAsync<Peer>();
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.FORM_("uk-card uk-card-primary");
                h.UL_("uk-card-body");
                if (o != null)
                {
                    h.LI_().FIELD("Peer ID", o.id)._LI();
                    h.LI_().FIELD("Name", o.Name)._LI();
                    h.LI_().FIELD("URI", o.Uri)._LI();
                }
                h._UL();
                h.FOOTER_("uk-card-footer uk-flex-center").TOOL(nameof(setg), css: "uk-button-secondary")._FOOTER();
                h._FORM();
            });
        }

        [Ui("Foreign", group: 2), Tool(Anchor)]
        public async Task foreign(WebContext wc)
        {
            using var dc = NewDbContext();
            dc.Sql("SELECT ").collst(Peer.Empty).T(" FROM chain.peers WHERE native = FALSE");
            var arr = await dc.QueryAsync<Peer>();
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

        [Ui("Setting", group: 1), Tool(ButtonShow)]
        public async Task setg(WebContext wc)
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
                
                // adjust in-memory model
                // ChainEnviron.ReloadNative();
                
                wc.GivePane(200); // close dialog
            }
        }

        [Ui("✛ New", group: 2), Tool(ButtonShow)]
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
                
                // reflect in-memory model
                
                
                wc.GivePane(200); // close dialog
            }
        }
    }
}