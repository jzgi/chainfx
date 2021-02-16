using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Db
{
    public class ChainVarWork : WebWork
    {
        [Ui("Modify", group: 1), Tool(ButtonOpen)]
        public async Task mod(WebContext wc)
        {
            short id = wc[0];

            var o = ChainEnviron.Clients[id]?.Info;

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
                dc.Sql("UPDATE chain.peers")._SET_(o).T(" WHERE id = @1");
                await dc.ExecuteAsync(p =>
                {
                    o.Write(p);
                    p.Set(id);
                });

                wc.GivePane(200); // close dialog
            }
        }
    }
}