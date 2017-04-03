using System.Collections.Generic;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    ///
    public abstract class ItemWork<V> : Work where V : ItemVarWork
    {
        public ItemWork(WorkContext fc) : base(fc)
        {
            CreateVar<ItemVarWork>();
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1 AND enabled", p => p.Set(shopid)))
                {
                    ac.Give(200, dc.Dump());
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }


        public void _(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM items WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.GiveWorkPage(Parent, 200, dc.ToList<Item>());
                }
                else
                {
                    ac.GiveWorkPage(Parent, 200, (List<Item>)null);
                }
            }
        }

        [Ui("新建", Mode = UiMode.AnchorDialog)]
        public async Task @new(ActionContext ac)
        {
            if (ac.GET)
            {
                Item o = Item.Empty;
                ac.GivePaneForm(200, o);
            }
            else // post
            {
                var item = await ac.ReadObjectAsync<Item>();
                item.shopid = ac[typeof(ShopVarWork)];
                using (var dc = Service.NewDbContext())
                {
                    dc.Sql("INSERT INTO items")._(Item.Empty)._VALUES_(Item.Empty)._("");
                    if (dc.Execute(p => p.Set(item)) > 0)
                    {
                        ac.Give(201); // created
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }

        public void _cat_(ActionContext ac)
        {
            string shopid = ac[1];
            using (var dc = ac.NewDbContext())
            {
                string name;
                int age;
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.GivePaneForm(303, dc, (i, o) =>
                {
                    o.Put(nameof(name), name = i.GetString());
                    o.Put(nameof(age), age = i.GetInt());
                }); // see other
            }
        }

        [Ui("上架/下架")]
        public void toggle(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                // ac.SetHeader();
                ac.Give(303); // see other
            }
        }

        [Ui("删除")]
        public async Task modify(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];

            if (ac.GET)
            {
                var item = new Item() { };
                ac.GivePaneForm(200, item);
            }
            else
            {
                var item = await ac.ReadObjectAsync<Item>();
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE items SET enabled = NOT enabled WHERE shopid = @1", p => p.Set(shopid));
                    // ac.SetHeader();
                    ac.Give(303); // see other
                }
            }
        }
    }

    public class OprItemWork : ItemWork<OprItemVarWork>
    {
        public OprItemWork(WorkContext wc) : base(wc)
        {
        }
    }
}