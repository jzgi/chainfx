using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Db
{
    public abstract class ChainMgtWork : WebWork
    {
        protected internal override void OnMake()
        {
            MakeVarWork<ChainVarWork>();
        }

        public virtual void @default(WebContext wc)
        {
            var o = ChainEnv.Info;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(caption: "本节点属性");

                h.UL_("uk-card uk-card-primary uk-card-body");
                if (o != null)
                {
                    h.LI_().FIELD("节点编号", o.Id)._LI();
                    h.LI_().FIELD("名称", o.Name)._LI();
                    h.LI_().FIELD("连接地址", o.Domain)._LI();
                    h.LI_().FIELD("状态", Peer.Statuses[o.status])._LI();
                    h.LI_().FIELD("当前区块", o.CurrentBlockId)._LI();
                }
                h._UL();

                h.TASKUL();
            });
        }

        [Ui("节点设置"), Tool(ButtonShow)]
        public virtual async Task setg(WebContext wc)
        {
            var o = ChainEnv.Info ?? new Peer
            {
                native = true,
            };
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("节点信息");
                    h.LI_().NUMBER("节点编号", nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().TEXT("名称", nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().URL("连接地址", nameof(o.domain), o.domain, max: 30, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, Peer.Statuses)._LI();
                    h._FIELDSUL()._FORM();
                });
            }
            else // POST
            {
                o = await wc.ReadObjectAsync(inst: o);
                using var dc = NewDbContext();
                dc.Sql("INSERT INTO chain.peers ").colset(o)._VALUES_(o).T(" ON CONFLICT (native) WHERE native = TRUE DO UPDATE SET ").setlst(o);
                await dc.ExecuteAsync(p => o.Write(p));

                if (ChainEnv.Info == null)
                {
                    ChainEnv.Info = o;
                }

                wc.GivePane(200); // close dialog
            }
        }
    }
}