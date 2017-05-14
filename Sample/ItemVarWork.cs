using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ItemVarWork : Work
    {
        public ItemVarWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[0];
            int id = ac[this];

            using (var dc = Service.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty)._("FROM orders WHERE id = @1 AND shopid = @2");
                if (dc.Query(p => p.Set(id).Set(shopid)))
                {
                    var order = dc.ToDatas<Order>();
                }
                else
                {
                }
            }
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            string name = ac[this];
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    var byteas = dc.GetByteAs();
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        StaticContent cont = new StaticContent(byteas);
                        ac.Give(200, cont, pub: true, maxage: 60 * 5);
                    }
                }
                else ac.Give(404, pub: true, maxage: 60 * 5); // not found
            }
        }
    }

    public class OprItemVarWork : ItemVarWork
    {
        public OprItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = UiMode.AnchorDialog)]
        public async Task edit(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ Item.ICON ^ Item.SHOPID;
                    dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1 AND name = @2");
                    if (dc.Query1(p => p.Set(shopid).Set(name)))
                    {
                        var o = dc.ToData<Item>(proj);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();

                            m.TEXT(nameof(o.name), o.name, label: "品名");
                            m.TEXT(nameof(o.descr), o.descr, label: "描述");
                            m.TEXT(nameof(o.unit), o.unit, label: "单位（如：斤，小瓶）");
                            m.NUMBER(nameof(o.price), o.price, label: "单价");
                            m.NUMBER(nameof(o.min), o.min, label: "起订数量（0表示不限）");
                            m.NUMBER(nameof(o.step), o.step, label: "递增因子");
                            m.NUMBER(nameof(o.qty), o.qty, label: "本批供应量");
                            m.SELECT(nameof(o.status), o.status, Item.STATUS);

                            m._FORM();
                        });
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
            else // post
            {
                const int proj = -1 ^ Item.SHOPID ^ Item.ICON;
                var o = await ac.ReadDataAsync<Item>(proj);
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj)._("WHERE shopid = @1 AND name = @2");
                    dc.Execute(p =>
                    {
                        o.WriteData(p, proj);
                        p.Set(shopid).Set(name);
                    });
                    ac.GivePane(200);
                }
            }
        }

        [Ui("照片", Mode = UiMode.AnchorCrop, Circle = true)]
        public new async Task icon(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        var byteas = dc.GetByteAs();
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else
                        {
                            StaticContent cont = new StaticContent(byteas);
                            ac.Give(200, cont);
                        }
                    }
                    else ac.Give(404); // not found           
                }
            }
            else // post
            {
                var frm = await ac.ReadAsync<Form>();
                ArraySegment<byte> icon = frm[nameof(icon)];
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE shopid = @2 AND name = @3", p => p.Set(icon).Set(shopid).Set(name)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
        }
    }
}