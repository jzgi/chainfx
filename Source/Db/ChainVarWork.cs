using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Db
{
    public class ChainVarWork : WebWork
    {
        [Ui("修改", group: 1), Tool(ButtonOpen)]
        public async Task upd(WebContext wc)
        {
            short id = wc[0];
            var o = Chain.GetClient(id)?.Info;
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("节点信息");
                    h.LI_().NUMBER("节点编号", nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().TEXT("名称", nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().URL("连接地址", nameof(o.domain), o.domain, max: 30, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, Peer.Statuses)._LI();
                    h._FIELDSUL().BOTTOM_BUTTON("确认", nameof(upd))._FORM();
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