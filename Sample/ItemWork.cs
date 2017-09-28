using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    public abstract class ItemWork<V> : Work where V : ItemVarWork
    {
        protected ItemWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, string>(obj => ((Item) obj).name);
        }
    }


    [Ui("货架")]
    [User(User.OPRAID)]
    public class OprItemWork : ItemWork<OprItemVarWork>
    {
        public OprItemWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            short shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = Item.SHOPID;
                dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1");
                if (dc.Query(p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Item>(proj), (h, o) => { });
                }
                else
                {
                    ac.GiveGridPage(200, (Item[]) null, null);
                }
            }
        }

        [Ui("新建产品", Mode = UiMode.AnchorShow)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                var o = new Item {min = 1, step = 1};
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, label: "品名", max: 10);
                    m.TEXT(nameof(o.descr), o.descr, label: "描述", max: 30);
                    m.TEXT(nameof(o.unit), o.unit, label: "单位（如：斤，小瓶）", required: true);
                    m.NUMBER(nameof(o.price), o.price, label: "单价", required: true);
                    m.NUMBER(nameof(o.min), o.min, label: "起订数量", min: (short) 1);
                    m.NUMBER(nameof(o.step), o.step, label: "递增因子", min: (short) 1);
                    m.NUMBER(nameof(o.max), o.max, label: "本批供应量");
                    m.SELECT(nameof(o.status), o.status, Item.STATUS);

                    m._FORM();
                });
            }
            else // post
            {
                var o = await ac.ReadObjectAsync<Item>();
                o.shopid = ac[typeof(ShopVarWork)];
                using (var dc = Service.NewDbContext())
                {
                    const int proj = Item.SHOPID;
                    dc.Sql("INSERT INTO items")._(Item.Empty, proj)._VALUES_(Item.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                    ac.GivePane(201);
                }
            }
        }

        [Ui("删除", Mode = UiMode.ButtonConfirm)]
        public async Task del(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            var f = await ac.ReadAsync<Form>();
            string[] key = f[nameof(key)];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("DELETE FROM items WHERE shopid = @1 AND name")._IN_(key);
                dc.Execute(p => p.Set(shopid));
                ac.GiveRedirect();
            }
        }
    }

    [Ui("产品")]
    [User(adm: true)]
    public class AdmItemWork : ItemWork<OprItemVarWork>
    {
        public AdmItemWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            short shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                const int proj = Item.SHOPID;
                dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1");
                if (dc.Query(p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Item>(proj), (h, o) => { });
                }
                else
                {
                    ac.GiveGridPage(200, (Item[]) null, null);
                }
            }
        }
    }
}