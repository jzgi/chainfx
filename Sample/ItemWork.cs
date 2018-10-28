using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class ItemWork : Work
    {
        protected ItemWork(WorkConfig cfg) : base(cfg)
        {
        }
    }


    [Ui("货架")]
    public class HublyItemWork : ItemWork
    {
        public HublyItemWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<HublyItemVarWork>();
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE hubid = @1 AND status > 0 ORDER BY status DESC");
                var arr = dc.Query<Item>(p => p.Set(hubid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                    {
                        var shops = Obtain<Map<short, Shop>>();
                        h.HEADER_("uk-card-header").T(o.name).SPAN(Item.Statuses[o.status], css: "uk-badge")._HEADER();
                        h.MAIN_("uk-card-body uk-flex");
                        h.PIC_(css: "uk-width-1-5").T(o.id).T("/icon")._PIC();
                        h.UL_(css: "uk-width-4-5 uk-padding-small-left");
                        h.LI_().FI("简　介", o.descr)._LI();
                        h.LI_().FI("单　位", o.unit).FI("冷　藏", o.refrig)._LI();
                        h.LI_().FI("价　格", o.price).FI("工坊用", o.shopp)._LI();
                        h.LI_().FI("派运用", o.senderp).FI("团组用", o.teamp)._LI();
                        h.LI_().FI("起　订", o.min).FI("递　增", o.step)._LI();
                        h.LI_().FI("周产能", o.cap7).FI("工　坊", shops[o.shopid]?.name)._LI();
                        h._UL();
                        h._MAIN();
                        h.VARTOOLS(css: "uk-card-footer uk-flex-between");
                    });
                });
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Item {min = 1, step = 1};
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDUL_("填写商品属性");
                    h.LI_().TEXT("品　名", nameof(o.name), o.name, max: 10, required: true)._LI();
                    h.LI_().TEXTAREA("简　介", nameof(o.descr), o.descr, max: 100, min: 20, required: true)._LI();
                    h.LI_().TEXTAREA("说　明", nameof(o.remark), o.descr, max: 500, min: 100, required: true)._LI();
                    h.LI_().TEXT("单　位", nameof(o.unit), o.unit, required: true).LABEL("冷　藏").CHECKBOX(nameof(o.refrig), o.refrig)._LI();
                    h.LI_().NUMBER("价　格", nameof(o.price), o.price, required: true).NUMBER("工坊用", nameof(o.shopp), o.shopp, required: true)._LI();
                    h.LI_().NUMBER("派运用", nameof(o.senderp), o.senderp, required: true).NUMBER("团组用", nameof(o.teamp), o.teamp, required: true)._LI();
                    h.LI_().NUMBER("起　订", nameof(o.min), o.min, min: (short) 1).NUMBER("递　增", nameof(o.step), o.step, min: (short) 1)._LI();
                    h._FIELDUL()._FORM();
                });
            }
            else // POST
            {
                const byte proj = Item.ID;
                var o = await wc.ReadObjectAsync<Item>();
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty, proj)._VALUES_(Item.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                wc.GivePane(200); // close dialog
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("删除", "删除所选货品吗？"), Tool(ButtonPickConfirm)]
        public async Task del(WebContext wc)
        {
            string orgid = wc[-1];
            var f = await wc.ReadAsync<Form>();
            string[] key = f[nameof(key)];
            if (key != null)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("DELETE FROM items WHERE orgid = @1 AND name")._IN_(key);
                    dc.Execute(p => p.Set(orgid), false);
                }
            }
            wc.GiveRedirect();
        }
    }
}