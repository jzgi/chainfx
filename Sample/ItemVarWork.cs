using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.GospelUtility;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    public abstract class ItemVarWork : Work
    {
        const int PICAGE = 60 * 60;

        protected ItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(ActionContext ac)
        {
            string shopid = ac[typeof(IShopVar)];
            string name = ac[this];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else ac.Give(200, new StaticContent(byteas), true, PICAGE);
                }
                else ac.Give(404, @public: true, maxage: PICAGE); // not found
            }
        }

        public void img(ActionContext ac, int ordinal)
        {
            string shopid = ac[-1];
            string name = ac[this];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) ac.Give(204); // no content 
                    else ac.Give(200, new StaticContent(byteas), true, PICAGE);
                }
                else ac.Give(404, @public: true, maxage: PICAGE); // not found
            }
        }
    }

    public class SampVarVarWork : ItemVarWork
    {
        public SampVarVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("购买"), Tool(ButtonOpen), Item('A')]
        public async Task add(ActionContext ac)
        {
            string shopid = ac[-1];
            var shop = Obtain<Map<string, Shop>>()[shopid];
            User prin = (User)ac.Principal;
            string itemname = ac[this];
            string name, city, a, b, tel; // form values
            short num;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    using (var dc = ac.NewDbContext())
                    {
                        h.FORM_();
                        if (dc.Scalar("SELECT 1 FROM orders WHERE wx = @1 AND status = 0 AND shopid = @2", p => p.Set(prin.wx).Set(shopid)) == null) // to create new
                        {
                            // show addr inputs for order creation
                            h.FIELDSET_("收货地址");
                            if (shop.areas != null) // dedicated areas
                            {
                                name = prin.name;
                                city = shop.city;
                                (a, b) = prin.addr.ToDual(SEPCHAR);
                                tel = prin.tel;
                                h.HIDDEN(nameof(name), name).HIDDEN(nameof(city), city);
                                h.SELECT(nameof(a), a, shop.areas, required: true, box: 4).TEXT(nameof(b), b, required: true, box: 8);
                                h.TEL(nameof(tel), tel, "电话", pattern: "[0-9]+", max: 11, min: 11, required: true);
                            }
                            else // free delivery
                            {
                                name = prin.name;
                                city = prin.city;
                                a = prin.addr;
                                tel = prin.tel;
                                h.SELECT(nameof(city), city, City.All, required: true, box: 3).TEXT(nameof(a), a, max: 20, required: true, box: 9);
                                h.TEXT(nameof(name), name, "姓名", max: 4, min: 2, required: true, box: 6).TEL(nameof(tel), tel, "电话", pattern: "[0-9]+", max: 11, min: 11, required: true, box: 6);
                            }
                            h._FIELDSET();
                        }
                        // quantity
                        h.FIELDSET_("加入购物车");
                        var it = dc.Query1<Item>(dc.Sql("SELECT ").columnlst(Item.Empty).T(" FROM items WHERE shopid = @1 AND name = @2"), p => p.Set(shopid).Set(itemname));
                        h.ICON("icon", box: 3).NUMBER(nameof(num), it.min, min: it.min, step: it.step, box: 7).FIELD(it.unit, box: 2);
                        h._FIELDSET();

                        h.FOOTBAR_().BUTTON("确定")._FOOTBAR();
                        h._FORM();
                    }
                });
            }
            else // POST
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT unit, price FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(itemname));
                    dc.Let(out string unit).Let(out decimal price);

                    if (dc.Query1("SELECT * FROM orders WHERE wx = @2 AND status = 0 AND shopid = @1", p => p.Set(shopid).Set(prin.wx))) // add to existing cart order
                    {
                        var o = dc.ToObject<Order>();
                        (await ac.ReadAsync<Form>()).Let(out num);
                        o.AddItem(itemname, unit, price, num);
                        o.TotalUp();
                        dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                    }
                    else // create a new order
                    {
                        var f = await ac.ReadAsync<Form>();
                        name = f[nameof(name)];
                        city = f[nameof(city)];
                        a = f[nameof(a)];
                        b = f[nameof(b)];
                        tel = f[nameof(tel)];
                        num = f[nameof(num)];
                        var o = new Order
                        {
                            rev = 1,
                            status = 0,
                            shopid = shopid,
                            shopname = shop.name,
                            typ = 0, // ordinal order
                            wx = prin.wx,
                            name = name,
                            city = city,
                            addr = shop.areas == null ? a : a + SEPCHAR + b, // concatenate addr if needed
                            tel = tel,
                            min = shop.min,
                            notch = shop.notch,
                            off = shop.off,
                            created = DateTime.Now
                        };
                        o.AddItem(itemname, unit, price, num);
                        o.TotalUp();
                        const byte proj = 0xff ^ Order.KEY ^ Order.LATER;
                        dc.Execute(dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj), p => o.Write(p, proj));
                    }
                    ac.GivePane(200, m =>
                    {
                        m.P("商品已经成功加入购物车");
                        m.FOOTBAR_().A_CLOSE("继续选购", true).A("去购物车付款", "/my//order/", true, targ: "_parent")._FOOTBAR();
                    });
                }
            }
        }
    }

    public class OprItemVarWork : ItemVarWork
    {
        public OprItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("基本"), Tool(ButtonShow, 2), User(OPRSTAFF)]
        public async Task basic(ActionContext ac)
        {
            string shopid = ac[-2];
            string name = ac[this];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE shopid = @1 AND name = @2", p => p.Set(shopid).Set(name));
                    ac.GivePane(200, m =>
                    {
                        m.FORM_();
                        m.FIELD(o.name, "名称");
                        m.TEXTAREA(nameof(o.descr), o.descr, "描述", min: 20, max: 50, required: true);
                        m.TEXT(nameof(o.unit), o.unit, "单位", required: true, box: 6).NUMBER(nameof(o.price), o.price, "单价", required: true, box: 6);
                        m.NUMBER(nameof(o.min), o.min, "起订", min: (short)1, box: 6).NUMBER(nameof(o.step), o.step, "增减", min: (short)1, box: 6);
                        m.SELECT(nameof(o.status), o.status, Item.Statuses, "状态", box: 6).NUMBER(nameof(o.stock), o.stock, "可供", box: 6);
                        m._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0xff ^ Item.PK;
                var o = await ac.ReadObjectAsync<Item>(proj);
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE shopid = @1 AND name = @2"), p =>
                    {
                        o.Write(p, proj);
                        p.Set(shopid).Set(name);
                    });
                }
                ac.GivePane(200); // close
            }
        }

        [Ui("照片"), Tool(ButtonCrop), User(OPRSTAFF)]
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
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE shopid = @2 AND name = @3", p => p.Set(jpeg).Set(shopid).Set(name)) > 0)
                    {
                        ac.Give(200); // ok
                    }
                    else ac.Give(500); // internal server error
                }
            }
        }

        [Ui("图示"), Tool(ButtonCrop, Ordinals = 4), User(OPRSTAFF)]
        public new async Task img(ActionContext ac, int ordinal)
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
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                ArraySegment<byte> jpeg = f[nameof(jpeg)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE items SET img" + ordinal + " = @1 WHERE shopid = @2 AND name = @3", p => p.Set(jpeg).Set(shopid).Set(name));
                }
                ac.Give(200); // ok
            }
        }

        [Ui("可供"), Tool(ButtonShow), User(OPRSTAFF)]
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
                    ac.GivePane(200, h => { h.FORM_().NUMBER(nameof(stock), stock, step: (short)1)._FORM(); });
                }
            }
            else // POST
            {
                (await ac.ReadAsync<Form>()).Let(out short stock);
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE items SET stock = @1 WHERE shopid = @2 AND name = @3", p => p.Set(stock).Set(shopid).Set(name));
                }
                ac.GivePane(200); // close
            }
        }
    }
}