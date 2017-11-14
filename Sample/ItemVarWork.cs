using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;

namespace Greatbone.Sample
{
    public class ItemCheckAttribute : CheckAttribute
    {
        readonly char state;

        public ItemCheckAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(object obj)
        {
            var o = obj as Item;
            switch (state)
            {
                case 'A': return o.status == 2;
            }
            return false;
        }
    }

    public abstract class ItemVarWork : Work
    {
        protected ItemVarWork(WorkContext wc) : base(wc)
        {
        }
    }


    public class PubItemVarWork : ItemVarWork
    {
        public PubItemVarWork(WorkContext wc) : base(wc)
        {
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[-1];
            string name = ac[this];
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else
                    {
                        ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                    }
                }
                else ac.Give(404, @public: true, maxage: 60 * 5); // not found
            }
        }

        [Ui("产品详情"), Style(AnchorOpen)]
        public void detail(ActionContext ac)
        {
        }

        [Ui("购买", "加入购物车"), Style(ButtonPrompt, 1), ItemCheck('A')]
        public async Task Add(ActionContext ac)
        {
            string shopid = ac[-1];
            string name = ac[this];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1 AND name = @2");
                    if (dc.Query1(p => p.Set(shopid).Set(name)))
                    {
                        var o = dc.ToObject<Item>();
                        ac.GivePane(200, h =>
                        {
                            h.FORM_();
                            h.NUMBER(nameof(o.max), o.min, min: o.min, step: o.step);
                            if (o.customs != null)
                            {
                                h.CHECKBOXGROUP(nameof(o.customs), null, o.customs, "附加要求");
                            }
                            h._FORM();
                        });
                    }
                }
                return;
            }


            User prin = (User) ac.Principal;
            var f = await ac.ReadAsync<Form>();
            string city = f[nameof(city)];
            string area = f[nameof(area)];
            string shopname = f[nameof(shopname)];
            decimal price = f[nameof(price)];
            short qty = f[nameof(qty)];
            string unit = f[nameof(unit)];
            string[] opts = f[nameof(opts)];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT id, items, total FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0", p => p.Set(shopid).Set(prin.wx)))
                {
                    var o = new Order();
                    dc.Let(out o.id).Let(out o.items).Let(out o.total);
                    o.AddItem(name, price, qty, unit, opts);
                    o.SetTotal();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                else
                {
                    var o = new Order
                    {
                        rev = 1,
                        shopid = shopid,
                        shopname = shopname,
                        wx = prin.wx,
                        name = prin.name,
                        tel = prin.tel,
                        city = city ?? prin.city,
                        area = area ?? prin.area,
                        addr = prin.addr,
                        items = new[] {new OrderItem {name = name, price = price, qty = qty, unit = unit, opts = opts}},
                    };
                    o.SetTotal();
                    const short proj = -1 ^ Order.ID ^ Order.LATER;
                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.Give(200);
            }
        }
    }

    public class OprItemVarWork : ItemVarWork
    {
        public OprItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改"), Style(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            short shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        var o = dc.ToObject<Item>();
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.FIELD(o.name, "名称");
                            m.TEXT(nameof(o.descr), o.descr, label: "简述", max: 30, required: true);
                            m.TEXT(nameof(o.unit), o.unit, label: "单位", required: true);
                            m.NUMBER(nameof(o.price), o.price, "单价", required: true);
                            m.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1);
                            m.NUMBER(nameof(o.step), o.step, "间隔", min: (short) 1);
                            m.NUMBER(nameof(o.max), o.max, "剩余");
                            m.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                            m._FORM();
                        });
                    }
                    else ac.Give(500); // internal server error
                }
            }
            else // post
            {
                const short proj = -1 ^ Item.UNMOD;
                var o = await ac.ReadObjectAsync<Item>(proj);
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE shopid = @1 AND name = @2");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(shopid).Set(name);
                    });
                }
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("图片"), Style(AnchorCrop, Circle = true)]
        public async Task icon(ActionContext ac)
        {
            string shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        ArraySegment<byte> byteas;
                        dc.Let(out byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else
                        {
                            ac.Give(200, new StaticContent(byteas));
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
                    else ac.Give(500); // internal server error
                }
            }
        }
    }
}