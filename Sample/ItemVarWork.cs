using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
{
    public abstract class ItemVarWork : Work
    {
        const int PIC_AGE = 60 * 15;

        protected ItemVarWork(WorkContext wc) : base(wc)
        {
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[typeof(IShopVar)];
            string name = ac[this];
            using (var dc = ServiceCtx.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else ac.Give(200, new StaticContent(byteas), true, PIC_AGE);
                }
                else ac.Give(404, @public: true, maxage: PIC_AGE); // not found
            }
        }

        public void img(ActionContext ac, int ordinal)
        {
            string shopid = ac[-1];
            string name = ac[this];
            using (var dc = ServiceCtx.NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else ac.Give(200, new StaticContent(byteas), true, PIC_AGE);
                }
                else ac.Give(404, @public: true, maxage: PIC_AGE); // not found
            }
        }
    }

    public class PubItemVarWork : ItemVarWork
    {
        public PubItemVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("购买"), Tool(ButtonShow), Item('A')]
        public async Task Add(ActionContext ac)
        {
            User prin = (User) ac.Principal;
            string shopid = ac[-1];
            string name = ac[this];
            short num;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var exist = dc.Scalar("SELECT 1 FROM orders WHERE wx = @1 AND status = 0 AND shopid = @2 LIMIT 1", p => p.Set(prin.wx).Set(shopid));
                    dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1 AND name = @2");
                    dc.Query1(p => p.Set(shopid).Set(name));
                    var o = dc.ToObject<Item>();
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        if (exist == null) // new needs more input
                        {
                            dc.Query1("SELECT city, areas FROM shops WHERE id = @1", p => p.Set(shopid));
                            dc.Let(out string city).Let(out string[] areas);
                            h.FIELDSET_("收货地址");
                            string tel;
                            if (areas != null) // limited delivery areas
                            {
                                ac.Query.Let(out string a).Let(out string b).Let(out string c).Let(out tel);
                                if (a == null) // init from principal
                                {
                                    (a, b, c) = prin.addr.To3Strings('\a');
                                    a = City.ResolveIn(a, areas);
                                    tel = prin.tel;
                                }
                                var sites = City.SitesOf(city, a);
                                b = City.ResolveIn(b, sites);
                                h.SELECT(nameof(a), a, areas, refresh: true, box: 4).SELECT(nameof(b), b, sites, box: 4).TEXT(nameof(c), c, box: 4);
                            }
                            else // formless address
                            {
                                ac.Query.Let(out string a).Let(out tel);
                                if (a == null) // init from principal
                                {
                                    a = prin.addr;
                                    tel = prin.tel;
                                }
                                h.TEXT(nameof(a), a, tip: "您的完整地址");
                            }
                            h.TEL(nameof(tel), tel, "您的随身电话", required: true);
                            h._FIELDSET();
                        }
                        // quantity
                        h.FIELDSET_("加入购物车");
                        h.ICON("icon", box: 3).NUMBER(nameof(num), o.min, min: o.min, step: o.step, box: 7).FIELD(o.unit, box: 2);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
                return;
            }
            // POST
            using (var dc = ac.NewDbContext())
            {
                dc.Query1("SELECT unit, price FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name));
                dc.Let(out string unit).Let(out decimal price);

                if (dc.Query1("SELECT * FROM orders WHERE  wx = @2 AND status = 0 AND shopid = @1", p => p.Set(shopid).Set(prin.wx)))
                {
                    var o = dc.ToObject<Order>();
                    (await ac.ReadAsync<Form>()).Let(out num);
                    o.AddItem(name, unit, price, num);
                    o.TotalUp();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                else // create new order
                {
                    var f = await ac.ReadAsync<Form>();
                    string a = f[nameof(a)];
                    string b = f[nameof(b)];
                    string c = f[nameof(c)];
                    string tel = f[nameof(tel)];
                    num = f[nameof(num)];
                    dc.Sql("SELECT ").columnlst(Shop.Empty).T(" FROM shops WHERE id = @1");
                    dc.Query1(p => p.Set(shopid).Set(prin.wx));
                    var shop = dc.ToObject<Shop>();
                    var o = new Order
                    {
                        rev = 1,
                        status = 0,
                        shopid = shopid,
                        shopname = shop.name,
                        pos = false,
                        wx = prin.wx,
                        name = prin.name,
                        tel = tel,
                        addr = shop.areas == null ? a : a + '\a' + b + '\a' + c, // concatenate addr if needed
                        min = shop.min,
                        notch = shop.notch,
                        off = shop.off,
                        created = DateTime.Now
                    };
                    o.AddItem(name, unit, price, num);
                    o.TotalUp();
                    const short proj = -1 ^ Order.KEY ^ Order.LATER;
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

        [Ui("基本"), Tool(ButtonShow, 2), User(OPRMEM)]
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

        [Ui("成品照"), Tool(ButtonCrop), User(OPRMEM)]
        public async Task ficon(ActionContext ac)
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
                using (var dc = ServiceCtx.NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE shopid = @2 AND name = @3", p => p.Set(jpeg).Set(shopid).Set(name)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else ac.Give(500); // internal server error
                }
            }
        }

        [Ui("过程照"), Tool(ButtonCrop, Ordinals = 4), User(OPRMEM)]
        public async Task fimgs(ActionContext ac, int ordinal)
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
                        else ac.Give(200, new StaticContent(byteas));
                    }
                    else ac.Give(404); // not found
                }
                return;
            }
            var f = await ac.ReadAsync<Form>();
            ArraySegment<byte> jpeg = f[nameof(jpeg)];
            using (var dc = ServiceCtx.NewDbContext())
            {
                dc.Execute("UPDATE items SET img" + ordinal + " = @1 WHERE shopid = @2 AND name = @3", p => p.Set(jpeg).Set(shopid).Set(name));
            }
            ac.Give(200); // ok
        }

        [Ui("供量"), Tool(ButtonShow), User(OPRMEM)]
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
                using (var dc = ServiceCtx.NewDbContext())
                {
                    dc.Execute("UPDATE items SET stock = @1 WHERE shopid = @2 AND name = @3", p => p.Set(stock).Set(shopid).Set(name));
                }
                ac.Give(200); // ok
            }
        }
    }
}