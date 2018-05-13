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


    [Ui("货品"), User(OPRMEM)]
    public class OprItemWork : ItemWork<OprItemVarWork>
    {
        public OprItemWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Item>("SELECT * FROM items WHERE orgid = @1 ORDER BY status DESC", p => p.Set(orgid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.ACCORDION(arr,
                        o =>
                        {
                            h.T("<section class=\"uk-accordion-title\">");
                            h.T(o.name);
                            h.T("</section>");
                            h.T("<section class=\"uk-accordion-content uk-grid\">");
                            h.PIC_(w: 0x14).T(o.name).T("/icon")._PIC();
                            h.COL_(0x34).P(o.descr, "描述").P_("单价").T("¥").T(o.price)._P().P_("佣金").T("¥").T(o.comp)._P()._COL();
                            h.P(o.unit, "单位", 0x14).P(o.min, "起订", 0x14).P(o.step, "递增", 0x14).P(o.stock, "存量", 0x14);
                            h.T("<hr>");
                            h.TOOLPAD();
                            h.T("</section>");
                        }, null);
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow), User(OPRMEM)]
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
                    h.NUMBER(nameof(o.comp), o.comp, "佣金", required: true);
                    h.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1);
                    h.NUMBER(nameof(o.step), o.step, "递增", min: (short) 1);
                    h.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                    h.NUMBER(nameof(o.stock), o.stock, "可供");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else // POST
            {
                var o = await wc.ReadObjectAsync<Item>();
                o.orgid = wc[-1];
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty)._VALUES_(Item.Empty);
                    dc.Execute(p => o.Write(p));
                }
                wc.GivePane(200); // close dialog
            }
        }

        [Ui("删除", "删除所选货品吗？"), Tool(ButtonPickConfirm), User(OPRMEM)]
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