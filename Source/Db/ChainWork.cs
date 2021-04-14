using System.Threading.Tasks;
using SkyChain.Web;
using static SkyChain.Web.Modal;

namespace SkyChain.Db
{
    [Ui("联盟链")]
    public class ChainWork : WebWork
    {
        protected internal override void OnCreate()
        {
            CreateVarWork<ChainVarWork>();
        }

        [Ui("本节点"), Tool(Anchor)]
        public virtual void @default(WebContext wc)
        {
            var o = ChainEnviron.Info;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(caption: "本节点属性");
                h.FORM_("uk-card uk-card-primary");
                h.UL_("uk-card-body");
                if (o != null)
                {
                    h.LI_().FIELD("节点编号", o.Id)._LI();
                    h.LI_().FIELD("名称", o.Name)._LI();
                    h.LI_().FIELD("连接地址", o.Uri)._LI();
                    h.LI_().FIELD("状态", ChainPeer.Statuses[o.status])._LI();
                    h.LI_().FIELD("当前区块", o.CurrentBlockId)._LI();
                }
                h._UL();
                h.FOOTER_("uk-card-footer uk-flex-center").TOOL(nameof(upd), css: "uk-button-secondary")._FOOTER();
                h._FORM();
            });
        }

        [Ui("外节点", group: 2), Tool(Anchor)]
        public virtual void foreign(WebContext wc)
        {
            var arr = ChainEnviron.Clients;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR(caption: "外节点管理");
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
                        h.LI_().FIELD("连接地址", o.Uri)._LI();
                        h.LI_().FIELD("状态", ChainPeer.Statuses[o.status])._LI();
                        h.LI_().FIELD("当前区块", o.CurrentBlockId)._LI();
                    }
                    h._UL();
                    h.FOOTER_("uk-card-footer uk-flex-center").VARTOOL(o.id, nameof(upd))._FOOTER();
                });
            });
        }

        [Ui("修改", group: 1), Tool(ButtonShow)]
        public virtual async Task upd(WebContext wc)
        {
            var o = ChainEnviron.Info ?? new ChainPeer
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
                    h.LI_().URL("连接地址", nameof(o.uri), o.uri, max: 30, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, ChainPeer.Statuses)._LI();
                    h._FIELDSUL()._FORM();
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

        [Ui("✚ 新建", group: 2), Tool(ButtonShow)]
        public virtual async Task @new(WebContext wc)
        {
            var o = new ChainPeer
            {
                native = false,
            };
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDSUL_("节点信息");
                    h.LI_().NUMBER("节点编号", nameof(o.id), o.id, min: 1, max: 24, required: true)._LI();
                    h.LI_().TEXT("名称", nameof(o.name), o.name, max: 20, required: true)._LI();
                    h.LI_().URL("连接地址", nameof(o.uri), o.uri, max: 30, required: true)._LI();
                    h.LI_().SELECT("状态", nameof(o.status), o.status, ChainPeer.Statuses)._LI();
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