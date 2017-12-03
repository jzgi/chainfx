using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Sample
{
    public class ItemlyAttribute : StateAttribute
    {
        readonly char state;

        public ItemlyAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(object obj)
        {
            var o = obj as Item;
            switch (state)
            {
                case 'A': return o.max > 0;
            }
            return false;
        }
    }

    public abstract class ItemVarWork : Work
    {
        protected ItemVarWork(WorkContext wc) : base(wc)
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
                    else ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                }
                else ac.Give(404, @public: true, maxage: 60 * 5); // not found
            }
        }
    }


    public class PubItemVarWork : ItemVarWork
    {
        public PubItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("产品详情"), Tool(AnchorOpen)]
        public void detail(ActionContext ac)
        {
            string shopid = ac[-1];
            string name = ac[this];

            ac.GivePage(200, m =>
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query("SELECT idx, descr FROM details WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name));
                    m.BOARDVIEW_();
                    while (dc.Next())
                    {
                        dc.Let(out int idx).Let(out string descr);
                        m.CARD_();
                        m.CAPTION(false, descr);
                        m.IMG(idx + "/img");
                        m._CARD();
                    }
                    m._BOARDVIEW();
                }
            });
        }

        [Ui("购买"), Tool(ButtonShow, 1), Itemly('A')]
        public async Task Add(ActionContext ac)
        {
            string shopid = ac[-1];
            string name = ac[this];
            User prin = (User) ac.Principal;

            string unit;
            decimal price;
            short qty;

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var exist = dc.Scalar("SELECT 1 FROM orders WHERE status = 0 AND wx = @1 AND shopid = @2 LIMIT 1", p => p.Set(prin.wx).Set(shopid));

                    dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1 AND name = @2");
                    dc.Query1(p => p.Set(shopid).Set(name));
                    var o = dc.ToObject<Item>();
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.HIDDEN(nameof(unit), o.unit);
                        h.HIDDEN(nameof(price), o.price);
                        if (exist == null)
                        {
                            h.FIELDSET_("请填写收货地址");
                            string addr = null;
                            h.TEXT(nameof(addr), addr, "地址");
                            h.TEL(nameof(addr), addr, "电话", required:true);
                            h._FIELDSET();
                        }
                        h.FIELDSET_("加入购物车");
                        h.THUMBNAIL("icon", box: 3).NUMBER(nameof(qty), o.min, min: o.min, step: o.step, box: 9);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
                return;
            }


            var f = await ac.ReadAsync<Form>();

            // from the dialog
            unit = f[nameof(unit)];
            price = f[nameof(price)];
            qty = f[nameof(qty)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty).T(" FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0");
                if (dc.Query1(p => p.Set(shopid).Set(prin.wx)))
                {
                    var o = dc.ToObject<Order>();
                    o.AddItem(name, price, qty, unit);
                    o.SetTotal();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                else
                {
                    dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                    dc.Query1(p => p.Set(shopid).Set(prin.wx));
                    var shop = dc.ToObject<Shop>();

                    var o = new Order
                    {
                        rev = 1,
                        shopid = shopid,
                        shopname = shop.name,
                        wx = prin.wx,
                        name = prin.name,
                        tel = prin.tel,
                        city = prin.city,
                        addr = prin.addr,
                        items = new[] {new OrderItem {name = name, price = price, qty = qty, unit = unit}},
                        min = shop.min,
                        notch = shop.notch,
                        off = shop.off
                    };
                    o.SetTotal();
                    const short proj = -1 ^ Order.ID ^ Order.LATER;
                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.GivePane(200);
            }
        }
    }

    public class OprItemVarWork : ItemVarWork
    {
        public OprItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(ActionContext ac)
        {
            string shopid = ac[-2];
            string name = ac[this];
            Item o;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT * FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name));
                    o = dc.ToObject<Item>();
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        m.FIELD(o.name, "名称", box: 6).TEXT(nameof(o.unit), o.unit, "单位", required: true, box: 6);
                        m.TEXTAREA(nameof(o.descr), o.descr, "描述", max: 30, required: true);
                        m.TEXT(nameof(o.content), o.content, "主料", required: true);
                        m.NUMBER(nameof(o.price), o.price, "单价", required: true, box: 6).NUMBER(nameof(o.min), o.min, "起订", min: (short) 1, box: 6);
                        m.NUMBER(nameof(o.step), o.step, "增减", min: (short) 1, box: 6).NUMBER(nameof(o.max), o.max, "数量", box: 6);
                        m.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                        m._FORM();
                    });
                }
                return;
            }
            const short proj = -1 ^ Item.UNMOD;
            o = await ac.ReadObjectAsync<Item>(proj);
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

        [Ui("图片"), Tool(ButtonCrop)]
        public new async Task icon(ActionContext ac)
        {
            string shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else ac.Give(200, new StaticContent(byteas));
                    }
                    else ac.Give(404); // not found           
                }
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                ArraySegment<byte> jpeg = f[nameof(jpeg)];
                using (var dc = Service.NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE shopid = @2 AND name = @3", p => p.Set(jpeg).Set(shopid).Set(name)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else ac.Give(500); // internal server error
                }
            }
        }
    }
}