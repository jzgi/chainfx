using System.Threading.Tasks;
using ChainFx.Web;
using static ChainFx.Web.Modal;
using static ChainFx.Fabric.Nodality;

namespace ChainFx.Fabric
{
    [Ui("平台联盟管理", icon: "social")]
    public class FedMgtWork : WebWork
    {
        protected internal override void OnCreate()
        {
            CreateVarWork<FedMgtVarWork>();
        }

        public void @default(WebContext wc)
        {
            var arr = Okayed;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(tip: Label);
                // h.BOARD(arr, ety =>
                // {
                //     var cli = ety.Value;
                //     var o = cli.Peer;
                //
                //     h.HEADER_("uk-card-header").T(o.name)._HEADER();
                //     h.UL_("uk-card-body");
                //     if (o != null)
                //     {
                //         h.LI_().FIELD("节点编号", o.Id)._LI();
                //         h.LI_().FIELD("名称", o.Name)._LI();
                //         h.LI_().FIELD("连接地址", o.Url)._LI();
                //         h.LI_().FIELD("状态", Info.Statuses[o.status])._LI();
                //         // h.LI_().FIELD("当前区块", o.CurrentBlockId)._LI();
                //     }
                //     h._UL();
                //     // h.FOOTER_("uk-card-footer uk-flex-center").VARTOOL(o.id, nameof(upd))._FOOTER();
                // });
            });
        }


        [Ui("✚", "添加联盟节点"), Tool(ButtonShow)]
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
                    h.LI_().URL("连接地址", nameof(o.weburl), o.weburl, max: 30, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, Entity.States)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                o = await wc.ReadObjectAsync(instance: o);

                using var nc = NewLdgrContext();
                // nc.InviteAsync(o);

                wc.GivePane(200); // close dialog
            }
        }
    }
}