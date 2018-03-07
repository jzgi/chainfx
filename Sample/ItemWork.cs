using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    public abstract class ItemWork<V> : Work where V : ItemVarWork
    {
        protected ItemWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Item) obj).name);
        }
    }


    [Ui("货品"), User(OPRSTAFF)]
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
                dc.Query("SELECT * FROM items WHERE orgid = @1 ORDER BY status DESC", p => p.Set(orgid));
                wc.GiveBoardPage(200, dc.ToArray<Item>(), (h,o) =>
                {
                    h.HEADER(o.name, Item.Statuses[o.status], o.status >= 1);
                    h.ICON(o.name + "/icon");
                    h.P(o.descr, "描述").P(o.price, "单价", "¥")._BOX();
                    h.FIELD(o.unit, "单位", width: 3).FIELD(o.min, "起订", width: 3).FIELD(o.step, "递增", width: 3).FIELD(o.stock, "存量", width: 3);
                    h.FOOTER();
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow, 2), User(OPRSTAFF)]
        public async Task @new(WebContext wc)
        {
            if (wc.GET)
            {
                var o = new Item {min = 1, step = 1};
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, label: "名称", max: 10, required: true);
                    m.TEXTAREA(nameof(o.descr), o.descr, "简述", min: 20, max: 50, required: true);
                    m.TEXT(nameof(o.unit), o.unit, "单位", required: true, width: 6).NUMBER(nameof(o.price), o.price, "单价", required: true, width: 6);
                    m.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1, width: 6).NUMBER(nameof(o.step), o.step, "递增", min: (short) 1, width: 6);
                    m.SELECT(nameof(o.status), o.status, Item.Statuses, "状态", width: 6).NUMBER(nameof(o.stock), o.stock, "可供", width: 6);
                    m._FORM();
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

        [Ui("删除", "删除所选货品吗？"), Tool(ButtonPickConfirm), User(OPRSTAFF)]
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