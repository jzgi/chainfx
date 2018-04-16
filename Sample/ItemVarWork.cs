using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class ItemVarWork : Work
    {
        const int PICAGE = 60 * 60;

        protected ItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext wc)
        {
            string orgid = wc[typeof(IOrgVar)];
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(byteas), true, PICAGE);
                }
                else wc.Give(404, @public: true, maxage: PICAGE); // not found
            }
        }

        public void img(WebContext wc, int ordinal)
        {
            string orgid = wc[-1];
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name)))
                {
                    dc.Let(out ArraySegment<byte> byteas);
                    if (byteas.Count == 0) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(byteas), true, PICAGE);
                }
                else wc.Give(404, @public: true, maxage: PICAGE); // not found
            }
        }
    }

    public class SampItemVarWork : ItemVarWork
    {
        public SampItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [User]
        [Ui("购买"), Tool(ButtonOpen, size: 1), Item('A')]
        public async Task buy(WebContext wc)
        {
            User prin = (User) wc.Principal;
            string orgid = wc[-1];
            string itemname = wc[this];
            var org = Obtain<Map<string, Org>>()[orgid];
            var item = Obtain<Map<(string, string), Item>>()[(orgid, itemname)];
            short num;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    using (var dc = NewDbContext())
                    {
                        h.FORM_();
                        if (dc.Scalar("SELECT 1 FROM orders WHERE status = 0 AND custwx = @1 AND orgid = @2", p => p.Set(prin.wx).Set(orgid)) == null) // to create new
                        {
                            // show addr inputs for order creation
                            h.FIELDSET_("填写收货信息");
                            h.TEXT(nameof(Order.custaddr), prin.addr, "地址", max: 20, required: true);
                            h.TEXT(nameof(Order.custname), prin.name, "姓名", max: 4, min: 2, required: true);
                            h.TEL(nameof(Order.custtel), prin.tel, "电话", pattern: "[0-9]+", max: 11, min: 11, required: true);
                            h._FIELDSET();
                        }
                        // quantity
                        h.FIELDSET_("加入购物车");
                        h.FIELD_("货品").ICON("icon", wid: 0x16)._T(item.name)._FIELD();
                        h.FIELD_("数量").NUMBER(nameof(num), item.min, min: item.min, max: item.stock, step: item.step)._T(item.unit)._FIELD();
                        h._FIELDSET();

                        h.BOTTOMBAR_().BUTTON("确定")._BOTTOMBAR();
                        h._FORM();
                    }
                });
            }
            else // POST
            {
                using (var dc = NewDbContext())
                {
                    // determine whether add to existing order or create new
                    if (dc.Query1("SELECT * FROM orders WHERE status = 0 AND custwx = @1 AND orgid = @2", p => p.Set(prin.wx).Set(orgid)))
                    {
                        var o = dc.ToObject<Order>();
                        (await wc.ReadAsync<Form>()).Let(out num);
                        o.AddItem(itemname, item.unit, item.price, item.comp, num);
                        dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2, net = @3 WHERE id = @4", p => p.Set(o.items).Set(o.total).Set(o.net).Set(o.id));
                    }
                    else // create a new order
                    {
                        const byte proj = 0xff ^ Order.KEY ^ Order.LATER;
                        var f = await wc.ReadAsync<Form>();
                        var o = new Order
                        {
                            orgid = orgid,
                            orgname = org.name,
                            custwx = prin.wx,
                            created = DateTime.Now
                        };
                        o.Read(f, proj);
                        num = f[nameof(num)];
                        o.AddItem(itemname, item.unit, item.price, item.comp, num);
                        dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                        dc.Execute(p => o.Write(p, proj));

                        // save user info
                        if (prin.name != o.custname || prin.tel != o.custtel || prin.addr != o.custaddr)
                        {
                            if (dc.Execute("INSERT INTO users (wx, name, tel, addr) VALUES (@1, @2, @3, @4) ON CONFLICT (wx) DO UPDATE SET name = @2, tel = @3, addr = @4", p => p.Set(o.custwx).Set(prin.name = o.custname).Set(prin.tel = o.custtel).Set(prin.addr = o.custaddr)) > 0)
                            {
                                wc.SetTokenCookie(prin, 0xff ^ CREDENTIAL); // refresh client token thru cookie
                            }
                        }
                    }
                    wc.GivePane(200, m =>
                    {
                        m.MSG_(true, "加入购物车成功", "商品已经成功加入购物车");
                        m.BOTTOMBAR_().A_CLOSE("继续选购", true).A("去购物车付款", "/my//ord/", true, targ: "_parent")._BOTTOMBAR();
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

        [Ui("修改"), Tool(ButtonShow), User(OPRMEM)]
        public async Task upd(WebContext wc)
        {
            string orgid = wc[-2];
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name));
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("填写货品信息");
                        h.STATIC(o.name, "名称");
                        h.TEXTAREA(nameof(o.descr), o.descr, "描述", min: 20, max: 50, required: true);
                        h.TEXT(nameof(o.unit), o.unit, "单位", required: true);
                        h.NUMBER(nameof(o.price), o.price, "单价", required: true);
                        h.NUMBER(nameof(o.comp), o.comp, "佣金", min: (decimal) 0.00, step: (decimal) 0.01);
                        h.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1);
                        h.NUMBER(nameof(o.step), o.step, "增减", min: (short) 1);
                        h.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                        h.NUMBER(nameof(o.stock), o.stock, "可供");
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0xff ^ Item.PK;
                var o = await wc.ReadObjectAsync<Item>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE orgid = @1 AND name = @2");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(orgid).Set(name);
                    });
                }
                wc.GivePane(200); // close
            }
        }

        [Ui("照片"), Tool(ButtonCrop), User(OPRMEM)]
        public new async Task icon(WebContext wc)
        {
            string orgid = wc[-2];
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT icon FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) wc.Give(204); // no content 
                        else wc.Give(200, new StaticContent(byteas));
                    }
                    else wc.Give(404); // not found           
                }
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE orgid = @2 AND name = @3", p => p.Set(img).Set(orgid).Set(name)) > 0)
                    {
                        wc.Give(200); // ok
                    }
                    else wc.Give(500); // internal server error
                }
            }
        }
    }
}