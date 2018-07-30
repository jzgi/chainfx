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
            CreateVar<V, string>(obj => ((Item) obj).name);
        }
    }


    [Ui("货品"), UserAccess(OPRMEM)]
    public class CtrItemWork : ItemWork<CtrItemVarWork>
    {
        public CtrItemWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string ctrid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Item>("SELECT * FROM items WHERE ctrid = @1 ORDER BY status DESC", p => p.Set(ctrid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                    {
                        h.T("<header class=\"uk-card-header\">");
                        h.T(o.name);
                        h.T("</header>");
                        h.T("<main class=\"uk-card-body uk-grid\">");
                        h.ICO_(css: "uk-width-1-4").T(o.name).T("/icon")._ICO();
                        h.UL_(css: "uk-width-3-4 uk-padding-small-left");
                        h.LI("描述", o.descr);
                        h.LI_().FI("单价", o.price).FI("供价", o._sup).FI("运费", o._dvr).FI("团费", o._grp)._LI();
                        h.LI_().FI("单位", o.unit).FI("起订", o.min).FI("递增", o.step).FI("存量", o.demand)._LI();
                        h._UL();
                        h.T("</main>");
                        h.VARTOOLS(css: "uk-card-footer");
                    }, null);
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow, Style.Primary), UserAccess(OPRMEM)]
        public async Task @new(WebContext wc)
        {
            if (wc.GET)
            {
                var o = new Item {min = 1, step = 1};
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("填写货品信息");
                    h.TEXT(nameof(o.name), o.name, label: "名称", max: 10, required: true);
                    h.TEXTAREA(nameof(o.descr), o.descr, "简述", min: 20, max: 50, required: true);
                    h.TEXT(nameof(o.unit), o.unit, "单位", required: true);
                    h.NUMBER(nameof(o.price), o.price, "单价", required: true);
                    h.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1);
                    h.NUMBER(nameof(o.step), o.step, "递增", min: (short) 1);
                    h.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                    h.NUMBER(nameof(o.demand), o.demand, "可供");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync<Item>();
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty)._VALUES_(Item.Empty);
                    dc.Execute(p => o.Write(p));
                }
                wc.GivePane(200); // close dialog
            }
        }

        [Ui("删除", "删除所选货品吗？"), Tool(ButtonPickConfirm), UserAccess(OPRMEM)]
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