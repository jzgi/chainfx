using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Samp
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
                case 'A': return o.stock > 0;
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

        public void img(ActionContext ac, int ordinal)
        {
            string shopid = ac[-1];
            string name = ac[this];
            using (var dc = Service.NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
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

        [Ui("购买"), Tool(ButtonShow, 1), Itemly('A')]
        public async Task Add(ActionContext ac)
        {
            // NOTE it can be a work order if current user is an operator

            User prin = (User) ac.Principal;
            string shopid = ac[-1];
            string name = ac[this];
            string unit;
            decimal price;
            short num;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var exist = (bool?) dc.Scalar("SELECT TRUE FROM orders WHERE status = 0 AND wx = @1 AND shopid = @2 LIMIT 1", p => p.Set(prin.wx).Set(shopid));
                    dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1 AND name = @2");
                    dc.Query1(p => p.Set(shopid).Set(name));
                    var o = dc.ToObject<Item>();
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        if (exist == true)
                        {
                            h.FIELD_().A("查看购物车", "/my//cart/", true)._FIELD();
                        }
                        else // new order, ask for necessary info
                        {
                            dc.Query1("SELECT city, areas FROM shops WHERE id = @1", p => p.Set(shopid));
                            dc.Let(out string city).Let(out string[] areas);
                            h.FIELDSET_("收货地址");
                            if (areas != null)
                            {
                                ac.Query.Let(out string a).Let(out string b).Let(out string c).Let(out string tel);
                                h.SELECT(nameof(a), a, areas, refresh: true, box: 4).SELECT(nameof(b), b, City.SitesOf(city, a), box: 4).TEXT(nameof(c), c, box: 4);
                                if (a == null)
                                {
//                                    (a, b, c) = prin.addr.To3Strings('\t');
                                }
                            }
                            else // formless address
                            {
                                h.TEXT(nameof(prin.addr), prin.addr, tip: "您的完整地址");
                            }
                            h.TEL(nameof(prin.tel), prin.tel, "您的随身电话", required: true);
                            h._FIELDSET();
                        }
                        //
                        h.FIELDSET_("加入购物车");
                        h.THUMBNAIL("icon", box: 3).NUMBER(nameof(num), o.min, min: o.min, step: o.step, box: 9);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
                return;
            }

            // from the dialog
            (await ac.ReadAsync<Form>()).Let(out unit).Let(out price).Let(out num);
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty).T(" FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0");
                if (dc.Query1(p => p.Set(shopid).Set(prin.wx)))
                {
                    var o = dc.ToObject<Order>();
                    o.AddItem(name, unit, price, num);
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
                        min = shop.min,
                        notch = shop.notch,
                        off = shop.off
                    };
                    o.AddItem(name, unit, price, num);
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

        [Ui("基本"), Tool(ButtonShow, 2)]
        public async Task basic(ActionContext ac)
        {
            string shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT * FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name));
                    var o = dc.ToObject<Item>();
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        m.FIELD(o.name, "名称");
                        m.TEXTAREA(nameof(o.descr), o.descr, "描述", min: 10, max: 30, required: true);
                        m.TEXT(nameof(o.content), o.content, "主含", min: 5, max: 20, required: true);
                        m.TEXT(nameof(o.unit), o.unit, "单位", required: true, box: 6).NUMBER(nameof(o.price), o.price, "单价", required: true, box: 6);
                        m.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1, box: 6).NUMBER(nameof(o.step), o.step, "增减", min: (short) 1, box: 6);
                        m.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                        m._FORM();
                    });
                }
            }
            else // POST
            {
                const short proj = -1 ^ Item.PK;
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
                ac.GivePane(200); // close
            }
        }

        [Ui("成品照"), Tool(ButtonCrop)]
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
            else // POST
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

        [Ui("过程照"), Tool(ButtonCrop, Ordinals = 4)]
        public async Task prep(ActionContext ac, int ordinal)
        {
            string shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) ac.Give(204); // no content 
                        else ac.Give(200, new StaticContent(byteas), true, 60 * 5);
                    }
                    else ac.Give(404, @public: true, maxage: 60 * 5); // not found
                }
                return;
            }
            var f = await ac.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = Service.NewDbContext())
            {
                dc.Execute("UPDATE items SET img" + ordinal + " = @1 WHERE shopid = @2 AND name = @3", p => p.Set(jpeg).Set(shopid).Set(name));
            }
            ac.Give(200); // ok
        }

        [Ui("供量"), Tool(ButtonShow)]
        public async Task max(ActionContext ac)
        {
            string shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT stock FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name));
                    dc.Let(out short stock);
                    ac.GivePane(200, h => { h.FORM_().NUMBER(nameof(stock), stock, step: (short) 1)._FORM(); });
                }
            }
            else // POST
            {
                (await ac.ReadAsync<Form>()).Let(out short stock);
                using (var dc = Service.NewDbContext())
                {
                    dc.Execute("UPDATE items SET stock = @1 WHERE shopid = @2 AND name = @3", p => p.Set(stock).Set(shopid).Set(name));
                }
                ac.Give(200); // ok
            }
        }
    }
}