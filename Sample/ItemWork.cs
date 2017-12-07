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


    [Ui("货品")]
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

        [Ui("新建"), Tool(ButtonShow)]
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
                    m.TEXT(nameof(o.unit), o.unit, label: "单位", required: true);
                    m.NUMBER(nameof(o.price), o.price, "单价", required: true);
                    m.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1);
                    m.NUMBER(nameof(o.step), o.step, "增减", min: (short) 1);
                    m.NUMBER(nameof(o.max), o.max, "剩余");
                    m.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
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

        [Ui("流动库存","当前流动库存状况"), Tool(AnchorOpen, 2)]
        public void pos(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = 1 AND direct", p => p.Set(shopid));
                var os = dc.ToArray<Order>();
                ac.GivePage(200, h =>
                {
                    for (int i = 0; i < os.Length; i++)
                    {
                        var o = os[i];
                        h.CARD_();
                        h.CAPTION_(false).T("单号")._T(o.id).SEP().T(o.paid)._CAPTION();
                        h.FIELD_("收货").T(o.name)._T(o.city)._T(o.addr)._FIELD();
                        for (int j = 0; j < o.items.Length; j++)
                        {
                            var oi = o.items[j];
                            h.FIELD(oi.name, box: 4).FIELD(oi.price, box: 4).FIELD(oi.qty, null, oi.unit, box: 4);
                        }
                        h.FIELD_(box: 8)._FIELD().FIELD(o.total, "总计", box: 4);
                        h.TAIL();
                        h._CARD();
                    }
                }, false, 3);
            }
        }
    }
}