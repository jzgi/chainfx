using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class ItemWork<V> : Work where V : ItemVarWork
    {
        protected ItemWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Item)obj).name);
        }
    }


    [Ui("货品"), UserAccess(CTR_SPR)]
    public class CtrItemWork : ItemWork<CtrItemVarWork>
    {
        public CtrItemWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Item>("SELECT * FROM items ORDER BY status DESC");
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                    {
                        h.HEADER_("uk-card-header").T(o.name).SPAN(Item.Statuses[o.status])._HEADER();
                        h.MAIN_("uk-card-body uk-flex");
                        h.ICO_(css: "uk-width-1-5").T(o.name).T("/icon")._ICO();
                        h.UL_(css: "uk-width-4-5 uk-padding-small-left");
                        h.LI_().FI("描述", o.descr)._LI();
                        h.LI_().FI("单价", o.price).FI("供应", o.giverp).FI("派送", o.dvrerp).FI("团组", o.grperp)._LI();
                        h.LI_().FI("单位", o.unit).FI("起订", o.min).FI("递增", o.step).FI("冷藏", o.refrig)._LI();
                        h._UL();
                        h._MAIN();
                        h.VARTOOLPAD(css: "uk-card-footer uk-flex-between");
                    });
                });
            }
        }

        [UserAccess(CTR_MGR)]
        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.GET)
            {
                var o = new Item { min = 1, step = 1 };
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_("填写货品信息");
                    h.LI_().TEXT(label: "品　名", name: nameof(o.name), v: o.name, max: 10, required: true)._LI();
                    h.LI_().TEXTAREA("简　介", nameof(o.descr), o.descr, max: 100, min: 20, required: true)._LI();
                    h.LI_().TEXTAREA("说　明", nameof(o.remark), o.descr, max: 500, min: 100, required: true)._LI();
                    h.LI_().URL("视　频", nameof(o.mov), o.mov)._LI();
                    h.LI_().TEXT("单　位", nameof(o.unit), o.unit, required: true)._LI();
                    h.LI_().NUMBER("单　价", nameof(o.price), o.price, required: true).LABEL("供应价").NUMBER(null, nameof(o.giverp), o.giverp, required: true)._LI();
                    h.LI_().NUMBER("派送费", nameof(o.dvrerp), o.dvrerp, required: true).LABEL("团组费").NUMBER(null, nameof(o.dvrerp), o.dvrerp, required: true)._LI();
                    h.LI_().NUMBER("起　订", nameof(o.min), o.min, min: (short)1).LABEL("增　减").NUMBER(null, nameof(o.step), o.step, min: (short)1)._LI();
                    h.LI_().LABEL("冷　藏").CHECKBOX(nameof(o.refrig), o.refrig)._LI();
                    h._FIELDUL();
                    h._FORM();
                });
            }
            else // POST
            {
                const byte proj = Item.PK;
                var o = await wc.ReadObjectAsync<Item>();
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty, proj)._VALUES_(Item.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                wc.GivePane(200); // close dialog
            }
        }

        [UserAccess(CTR_MGR)]
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