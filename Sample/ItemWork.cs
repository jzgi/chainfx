using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class ItemWork<V> : Work where V : ItemVarWork
    {
        protected ItemWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, string>(obj => ((Item) obj).name);
        }
    }


    [Ui("货架")]
    [User(User.OPR_)]
    public class OprItemWork : ItemWork<OprItemVarWork>
    {
        public OprItemWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            short shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Item>(), (h, o) =>
                    {
                        h.CELL(o.name, "名称", 6).CELL(o.name, "名称", 6);
                        h.CELL(o.descr, "简述");
                        h.CELL(o.price, "价格");
                        h.CELL(o.name, "成分");
                    });
                }
                else
                {
                    ac.GiveGridPage(200, (Item[]) null, null);
                }
            }
        }

        [Ui("新建", Mode = UiMode.ButtonShow)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                var o = new Item {min = 1, step = 1};
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.TEXT(nameof(o.descr), o.descr, "简述", max: 30, required: true);
                    m.TEXT(nameof(o.unit), o.unit, "单位", required: true);
                    m.NUMBER(nameof(o.price), o.price, "单价", required: true);
                    m.NUMBER(nameof(o.min), o.min, "起订数量", min: (short) 1);
                    m.NUMBER(nameof(o.step), o.step, "增减间隔", min: (short) 1);
                    m.NUMBER(nameof(o.max), o.max, "剩余供给");
                    m.SELECT(nameof(o.status), o.status, Item.STATUS, "状态");
                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Item>();
                o.shopid = ac[-1];
                using (var dc = Service.NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty)._VALUES_(Item.Empty);
                    dc.Execute(p => o.Write(p));
                }
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("删除", Mode = UiMode.ButtonConfirm)]
        public async Task del(ActionContext ac)
        {
            short shopid = ac[-1];
            var f = await ac.ReadAsync<Form>();
            string[] key = f[nameof(key)];
            if (key != null)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("DELETE FROM items WHERE shopid = @1 AND name")._IN_(key);
                    dc.Execute(p => p.Set(shopid));
                }
            }
            ac.GiveRedirect();
        }
    }
}