using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Db
{
    [Ui("&#128160;")]
    public class ChainWork : WebWork
    {
        protected internal override void OnCreate()
        {
            CreateVarWork<ChainVarWork>();
        }

        [Ui("Native"), Tool(Anchor)]
        public virtual void @default(WebContext wc)
        {
            var o = ChainEnviron.Info;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.FORM_("uk-card uk-card-primary");
                h.UL_("uk-card-body");
                if (o != null)
                {
                    h.LI_().FIELD("Peer ID", o.Id)._LI();
                    h.LI_().FIELD("Name", o.Name)._LI();
                    h.LI_().FIELD("URI", o.Uri)._LI();
                    h.LI_().FIELD("Status", ChainPeer.Statuses[o.status])._LI();
                    h.LI_().FIELD("Block #", o.CurrentBlockId)._LI();
                }
                h._UL();
                h.FOOTER_("uk-card-footer uk-flex-center").TOOL(nameof(mod), css: "uk-button-secondary")._FOOTER();
                h._FORM();
            });
        }

        [Ui("Foreign", group: 2), Tool(Anchor)]
        public void foreign(WebContext wc)
        {
            var arr = ChainEnviron.Clients;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.BOARD(arr, ety =>
                {
                    var cli = ety.Value;
                    var o = cli.Info;

                    h.HEADER_("uk-card-header").T(o.name)._HEADER();
                    h.UL_("uk-card-body");
                    if (o != null)
                    {
                        h.LI_().FIELD("Peer ID", o.Id)._LI();
                        h.LI_().FIELD("Name", o.Name)._LI();
                        h.LI_().FIELD("URI", o.Uri)._LI();
                        h.LI_().FIELD("Status", ChainPeer.Statuses[o.status])._LI();
                        h.LI_().FIELD("Block #", o.CurrentBlockId)._LI();
                    }
                    h._UL();
                    h.FOOTER_("uk-card-footer uk-flex-center").VARTOOL(o.id, nameof(mod))._FOOTER();
                });
            });
        }

        [Ui("Modify", group: 1), Tool(ButtonOpen)]
        public async Task mod(WebContext wc)
        {
            var o = ChainEnviron.Info ?? new ChainPeer
            {
                native = true,
            };

            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("Peer Info");
                    h.LI_().NUMBER("ID", nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().TEXT("Name", nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().URL("Url", nameof(o.uri), o.uri, max: 30, required: true)._LI();
                    h.LI_().SELECT("Status", nameof(o.status), o.status, ChainPeer.Statuses)._LI();
                    h._FIELDSUL().BOTTOM_BUTTON("Save", nameof(mod))._FORM();
                });
            }
            else // POST
            {
                o = await wc.ReadObjectAsync(inst: o);
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.peers ").colset(o)._VALUES_(o).T(" ON CONFLICT (native) WHERE native = TRUE DO UPDATE SET ").setlst(o);
                await dc.ExecuteAsync(p => o.Write(p));

                if (ChainEnviron.Info == null)
                {
                    ChainEnviron.Info = o;
                }

                wc.GivePane(200); // close dialog
            }
        }

        [Ui("⊹ New", group: 2), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            var o = new ChainPeer
            {
                native = false,
            };
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("Peer Info");
                    h.LI_().NUMBER("ID", nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().TEXT("Name", nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().URL("Url", nameof(o.uri), o.uri, max: 30, required: true)._LI();
                    h.LI_().SELECT("Status", nameof(o.status), o.status, ChainPeer.Statuses)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                o = await wc.ReadObjectAsync(inst: o);
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.peers ").colset(o)._VALUES_(o);
                await dc.ExecuteAsync(p => o.Write(p));

                var cli = new ChainClient(o);
                await o.PeekLastBlockAsync(dc);
                ChainEnviron.Clients.Add(cli);

                wc.GivePane(200); // close dialog
            }
        }
    }
}