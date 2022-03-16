using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Nodal
{
    [Ui("平台联盟节点管理", "social")]
    public class NodalWork : WebWork
    {
        protected internal override void OnCreate()
        {
            CreateVarWork<NodalVarWork>();
        }

        public void @default(WebContext wc)
        {
            var arr = Home.Connectors;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(tip: "平台联盟节点管理");
                h.BOARD(arr, ety =>
                {
                    var cli = ety.Value;
                    var o = cli.Info;

                    h.HEADER_("uk-card-header").T(o.name)._HEADER();
                    h.UL_("uk-card-body");
                    if (o != null)
                    {
                        h.LI_().FIELD("节点编号", o.Id)._LI();
                        h.LI_().FIELD("名称", o.Name)._LI();
                        h.LI_().FIELD("连接地址", o.Domain)._LI();
                        h.LI_().FIELD("状态", Info.Statuses[o.status])._LI();
                        h.LI_().FIELD("当前区块", o.CurrentBlockId)._LI();
                    }
                    h._UL();
                    // h.FOOTER_("uk-card-footer uk-flex-center").VARTOOL(o.id, nameof(upd))._FOOTER();
                });
            });
        }


        [Ui("✚", "新建盟友"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            var o = new Peer
            {
            };
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("盟友信息");
                    h.LI_().NUMBER("编号", nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().TEXT("平台名称", nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().URL("连接地址", nameof(o.domain), o.domain, max: 30, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, Info.Statuses)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                o = await wc.ReadObjectAsync(instance: o);
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.peers ").colset(o)._VALUES_(o);
                await dc.ExecuteAsync(p => o.Write(p));

                var cli = new NodeClient(o);
                await o.PeekLastBlockAsync(dc);
                Home.Connectors.Add(cli);

                wc.GivePane(200); // close dialog
            }
        }
    }
}