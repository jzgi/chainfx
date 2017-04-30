using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class ItemVarWork : Work
    {
        protected ItemVarWork(WorkContext wc) : base(wc)
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
                    var order = dc.ToArray<Order>();
                }
                else
                {
                }
            }
        }

        public void _icon_(ActionContext ac)
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
                        ac.Give(200, cont, true, 60);
                    }
                }
                else ac.Give(404); // not found           
            }
        }
    }

    public class PubItemVarWork : ItemVarWork
    {
        public PubItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("加入购物车", UiMode.ButtonDialog)]
        public async Task add(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT unit, price, min, step FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        var unit = dc.GetString();
                        var price = dc.GetDecimal();
                        var min = dc.GetShort();
                        short qty = min;
                        var step = dc.GetShort();
                        ac.GiveFormPane(200, h =>
                        {
                            h.Add(name);
                            h.NUMBER(nameof(qty), qty, label: "数量", min: min, step: step, required: true);
                            h.HIDDEN(nameof(unit), unit);
                            h.HIDDEN(nameof(price), price);
                        });
                    }
                    else ac.Give(404); // not found           
                }
            }
            else // process post
            {
                var frm = await ac.ReadAsync<Form>();
                short qty = frm[nameof(qty)];
                string unit = frm[nameof(unit)];
                decimal price = frm[nameof(price)];
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT detail, total FROM orders WHERE shopid = @1 AND custwx = @2 AND status = 0", p => p.Set(shopid).Set(shopid)))
                    {
                        var order = new Order();
                        order.detail = dc.GetArray<OrderLine>();
                        order.total = dc.GetDecimal();
                        order.AddItem(name, qty, unit, price);
                        dc.Execute("UPDATE orders SET detail = @1, total = @2 WHERE ", p => p.Set(order.detail).Set(order.total));
                    }
                    else
                    {
                        string shop = (string) dc.Scalar("SELECT name FROM shops WHERE id = @1", p => p.Set(shopid));
                        User prin = (User) ac.Principal;
                        var order = new Order
                        {
                            shopid = shopid,
                            shop = shop,
                            custname = prin.name,
                            custwx = prin.wx,
                            custtel = prin.tel,
                            custcity = prin.city,
                            custdistr = prin.distr,
                            custaddr = prin.addr,
                            detail = new[]
                            {
                                new OrderLine {item = name, price = price, qty = qty, unit = ""}
                            }
                        };
                        order.Sum();

                        const int proj = -1 ^ Order.ID ^ Order.LATE;

                        dc.Sql("INSERT INTO orders ")._(order, proj)._VALUES_(order, proj);
                        dc.Execute(p => order.WriteData(p, proj));
                    }
                }
            }
        }
    }

    public class OprItemVarWork : ItemVarWork
    {
        public OprItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", UiMode.AnchorDialog)]
        public async Task edit(ActionContext ac)
        {
            if (ac.GET)
            {
                string shopid = ac[typeof(ShopVarWork)];
                string name = ac[this];
                using (var dc = Service.NewDbContext())
                {
                    const int proj = -1 ^ Item.ICON ^ Item.SHOPID;
                    dc.Sql("SELECT ").columnlst(Item.Empty, proj)._("FROM items WHERE shopid = @1 AND name = @2");
                    if (dc.Query1(p => p.Set(shopid).Set(name)))
                    {
                        ac.GiveFormPane(200, dc.ToObject<Item>(proj), proj);
                    }
                    else
                    {
                        ac.Give(500); // internal server error
                    }
                }
            }
            else // post
            {
                var item = await ac.ReadObjectAsync<Item>();
                item.shopid = ac[typeof(ShopVarWork)];
                using (var dc = Service.NewDbContext())
                {
                    const int proj = -1 ^ Item.ICON;
                    dc.Sql("INSERT INTO items")._(Item.Empty, proj)._VALUES_(Item.Empty, proj)._("");
                    if (dc.Execute(p => item.WriteData(p, proj)) > 0)
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

        [Ui("图片", UiMode.AnchorCrop, Circle = true)]
        public async Task icon(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = Service.NewDbContext())
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