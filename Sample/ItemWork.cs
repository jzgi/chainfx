using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Samp
{
    public abstract class ItemWork<V> : Work where V : ItemVarWork
    {
        protected ItemWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, string>(obj => ((Item) obj).name);
        }
    }


    [Ui("货架")]
    public class OprItemWork : ItemWork<OprItemVarWork>
    {
        public OprItemWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM items WHERE shopid = @1 ORDER BY status DESC", p => p.Set(shopid));
                ac.GiveBoardPage(200, dc.ToArray<Item>(), (h, o) =>
                {
                    h.CAPTION(false, o.name, Item.Statuses[o.status], o.status >= Item.ON);
                    h.IMG(o.name + "/icon", box: 3);
                    h.BOX_(0x49).P(o.descr, "描述").P(o.content, "主含").P(o.price, "价格")._BOX();
                    h.FIELD(o.unit, "单位", box: 3).FIELD(o.min, "起订", box: 3).FIELD(o.step, "增减", box: 3).FIELD(o.max, "数量", box: 3);
                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow, 2)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                var o = new Item {min = 1, step = 1};
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, label: "名称", max: 10, required: true);
                    m.TEXTAREA(nameof(o.descr), o.descr, "简述", max: 30, required: true);
                    m.TEXT(nameof(o.content), o.content, label: "主含", max: 10, required: true);
                    m.TEXT(nameof(o.unit), o.unit, label: "单位", required: true, box: 6).NUMBER(nameof(o.price), o.price, "单价", required: true, box: 6);
                    m.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1, box: 6).NUMBER(nameof(o.step), o.step, "增减", min: (short) 1, box: 6);
                    m.NUMBER(nameof(o.max), o.max, "剩余", box: 6).SELECT(nameof(o.status), o.status, Item.Statuses, "状态", box: 6);
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

        [Ui("删除"), Tool(ButtonConfirm)]
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

        [Ui("移动销售", "当前移动销售状况"), Tool(AnchorOpen, 2)]
        public void mosale(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = 1 AND mosale", p => p.Set(shopid));
                var os = dc.ToArray<Order>();
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    for (int i = 0; i < os.Length; i++)
                    {
                        var o = os[i];
                        h.CAPTION_(false).T(o.addr)._T(o.id).SEP().T(o.paid)._CAPTION(o.name);
                        for (int j = 0; j < o.items.Length; j++)
                        {
                            var oi = o.items[j];
                            h.FIELD(oi.name, box: 4).FIELD(oi.price, box: 4).FIELD(oi.qty, null, oi.unit, box: 4);
                        }
                    }
                    h._FORM();
                }, false, 3);
            }
        }
    }
}