using System.Threading.Tasks;
using Greatbone.Core;
using static System.Data.IsolationLevel;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.Order;
using static Greatbone.Samp.SampUtility;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
{
    public abstract class OrderVarWork : Work
    {
        protected OrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MyOrderVarWork : OrderVarWork
    {
        public MyOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("付款¥", flag: 1), Tool(ButtonScript), Order('P')]
        public async Task prepay(ActionContext ac)
        {
            string wx = ac[-2];
            int orderid = ac[this];
            short rev;
            decimal total;
            User prin = (User) ac.Principal;
            using (var dc = ac.NewDbContext())
            {
                dc.Query1("SELECT rev, total, typ, name, city, addr, tel FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                dc.Let(out rev).Let(out total).Let(out short typ).Let(out prin.name).Let(out prin.city).Let(out prin.addr).Let(out prin.tel);
                if (typ == 0) // normal order then keep user info
                {
                    if (dc.Execute("INSERT INTO users (wx, name, city, addr, tel) VALUES (@1, @2, @3, @4, @5) ON CONFLICT (wx) DO UPDATE SET name = COALESCE(@2, users.name), city = COALESCE(@3, users.city), addr = COALESCE(@4, users.addr), tel = COALESCE(@5, users.tel)", p => p.Set(wx).Set(prin.name).Set(prin.city).Set(prin.addr).Set(prin.tel)) > 0)
                    {
                        ac.SetTokenCookie(prin, 0xff ^ CREDENTIAL); // refresh client token thru cookie
                    }
                }
            }
            var (prepay_id, _) = await WeiXinUtility.PostUnifiedOrderAsync(orderid + "-" + rev, total, wx, ac.RemoteAddr, NETADDR + "/paynotify", "粗粮达人-健康产品");
            if (prepay_id != null)
            {
                ac.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
            }
            else
            {
                ac.Give(500);
            }
        }

        [Ui("修改", flag: 8), Tool(ButtonShow)]
        public async Task addr(ActionContext ac)
        {
            int orderid = ac[this];
            string wx = ac[-2];
            string name, city, a, b, tel; // form values
            if (ac.GET)
            {
                var shops = Obtain<Map<string, Shop>>();
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query1("SELECT shopid, name, city, addr, tel FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                        dc.Let(out string oshopid).Let(out string oname).Let(out string ocity).Let(out string oaddr).Let(out string otel);
                        h.FIELDSET_("收货地址");
                        var shop = shops[oshopid];
                        if (shop.areas != null) // limited delivery areas
                        {
                            name = oname;
                            city = ocity;
                            (a, b) = oaddr.ToDual(SEPCHAR);
                            tel = otel;
                            h.HIDDEN(nameof(name), name).HIDDEN(nameof(city), city);
                            h.SELECT(nameof(a), a, shop.areas, required: true, box: 4).TEXT(nameof(b), b, required: true, box: 8);
                            h.TEL(nameof(tel), tel, "电话", pattern: "[0-9]+", max: 11, min: 11, required: true);
                        }
                        else // free delivery
                        {
                            name = oname;
                            city = ocity;
                            a = oaddr;
                            tel = otel;
                            h.SELECT(nameof(city), city, City.All, box: 3).TEXT(nameof(a), a, max: 20, required: true, box: 9);
                            h.TEXT(nameof(name), name, "姓名", max: 4, min: 2, required: true, box: 6).TEL(nameof(tel), tel, "电话", pattern: "[0-9]+", max: 11, min: 11, required: true, box: 6);
                        }
                        h._FIELDSET();
                    }
                    h._FORM();
                });
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                name = f[nameof(name)];
                city = f[nameof(city)];
                a = f[nameof(a)];
                b = f[nameof(b)]; // can be null indicating free delivery
                tel = f[nameof(tel)];
                using (var dc = ac.NewDbContext())
                {
                    string addr = b == null ? a : a + SEPCHAR + b;
                    dc.Execute("UPDATE orders SET name = @1, city = @2, addr = @3, tel = @4 WHERE id = @5", p => p.Set(name).Set(city).Set(addr).Set(tel).Set(orderid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("修改", flag: 8), Tool(ButtonShow)]
        public async Task item(ActionContext ac, int idx)
        {
            int orderid = ac[this];
            string wx = ac[-2];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    var oi = o.items[idx];
                    dc.Query1("SELECT step, stock FROM items WHERE shopid = @1 AND name = @2", p => p.Set(o.shopid).Set(oi.name));
                    dc.Let(out short step).Let(out short stock);
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("购买数量");
                        h.ICON("/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                        h.NUMBER(nameof(oi.qty), oi.qty, min: (short) 0, max: stock, step: step, box: 8);
                        h.FIELD(oi.unit, box: 2);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                short qty = f[nameof(qty)];
                using (var dc = ac.NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    o.UpdItem(idx, qty);
                    o.TotalUp();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                ac.GivePane(200);
            }
        }

        //        [Ui("建议"), Tool(ButtonShow)]
        public async Task kick(ActionContext ac)
        {
            int orderid = ac[this];
            string kick = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.FIELDSET_("我的建议");
                    m.TEXTAREA(nameof(kick), kick, null, max: 40, required: true);
                    m._FIELDSET();
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                kick = f[nameof(kick)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET kick = @1 WHERE id = @2", p => p.Set(kick).Set(orderid));
                }
                ac.GivePane(200);
            }
        }
    }

    public class OprCartVarWork : OrderVarWork
    {
        public OprCartVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("加货"), Tool(ButtonShow, 2), User(OPRSTAFF)]
        public async Task add(ActionContext ac)
        {
            string shopid = ac[-2];
            int orderid = ac[this];
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query("SELECT name, unit, price, stock FROM items WHERE shopid = @1 AND status > 0 AND stock > 0", p => p.Set(shopid));
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out string unit).Let(out decimal price).Let(out short stock);
                            m.FIELD(name, box: 5).FIELD(stock, fix: unit, box: 0x22).NUMBER(name + '~' + unit + '~' + price, (short) 0, min: (short) 0, step: (short) 1, max: stock, box: 5);
                        }
                    }
                    m._FORM();
                });
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                using (var dc = ac.NewDbContext(ReadUncommitted))
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1", p => p.Set(orderid));
                    for (int i = 0; i < f.Count; i++)
                    {
                        var e = f.At(i);
                        var (name, unit, price) = e.Key.ToTriple('~');
                        short n = e.Value;
                        if (n != 0) o.ReceiveItem(name, unit, decimal.Parse(price), n);
                    }
                    dc.Execute("UPDATE orders SET items = @1 WHERE id = @2", p => p.Set(o.items).Set(o.id));
                }
                ac.GivePane(200);
            }
        }

        [Ui("分派"), Tool(ButtonShow), User(OPRSTAFF)]
        public async Task assign(ActionContext ac)
        {
            int orderid = ac[this];
            string shopid = ac[-2];
            string opr;
            string addr = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        // select operator info
                        dc.Query("SELECT wx, name, tel FROM users WHERE oprat = @1", p => p.Set(shopid));
                        m.SELECT_(nameof(opr), "人员");
                        while (dc.Next())
                        {
                            dc.Let(out string wx).Let(out string name).Let(out string tel);
                            m.OPTION(wx + '~' + name + '~' + tel, name);
                        }
                        m._SELECT();
                        // input addr
                        var shop = Obtain<Map<string, Shop>>()[shopid];
                        if (shop.areas != null)
                        {
                            m.SELECT(nameof(addr), addr, shop.areas, "区域");
                        }
                        else
                        {
                            m.TEXT(nameof(addr), addr, "区域");
                        }
                    }
                    m._FORM();
                });
            }
            else
            {
                (await ac.ReadAsync<Form>()).Let(out opr).Let(out addr);
                var (wx, name, tel) = opr.ToTriple('~');
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET wx = @1, name = @2, tel = @3, addr = @4 WHERE id = @5 AND shopid = @6",
                        p => p.Set(wx).Set(name).Set(tel).Set(addr).Set(orderid).Set(shopid));
                }
                ac.GivePane(200);
            }
        }
    }

    public class OprNewlyVarWork : OrderVarWork
    {
        public OprNewlyVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("撤消", "【警告】确认要撤销此单吗？实收金额将退回给买家"), Tool(ButtonConfirm)]
        public async Task abort(ActionContext ac)
        {
            string shopid = ac[-2];
            int orderid = ac[this];
            short rev = 0;
            decimal total = 0, cash = 0;
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT rev, total, cash FROM orders WHERE id = @1 AND status = " + PAID, p => p.Set(orderid)))
                {
                    dc.Let(out rev).Let(out total).Let(out cash);
                }
            }
            if (cash > 0)
            {
                string err = await WeiXinUtility.PostRefundAsync(orderid + "-" + rev, total, cash);
                if (err == null) // success
                {
                    using (var dc = ac.NewDbContext(ReadUncommitted))
                    {
                        dc.Query1("UPDATE orders SET status = " + ABORTED + ", aborted = localtimestamp WHERE id = @1 AND shopid = @2 RETURNING items", p => p.Set(orderid).Set(shopid));
                        dc.Let(out OrderItem[] items);
                        for (int i = 0; i < items?.Length; i++) // revert stock
                        {
                            var oi = items[i];
                            dc.Execute("UPDATE items SET stock = stock + @1 WHERE shopid = @2 AND name = @3", p => p.Set(oi.qty).Set(shopid).Set(oi.name));
                        }
                    }
                }
            }
            ac.GiveRedirect("../");
        }

        [Ui("完成"), Tool(ButtonShow)]
        public async Task deliver(ActionContext ac)
        {
            string shopid = ac[-2];
            int orderid = ac[this];
            User prin = (User) ac.Principal;
            bool mycart;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    // check personal pos
                    using (var dc = ac.NewDbContext())
                    {
                        m.FORM_();
                        if (dc.Query1("SELECT TRUE FROM orders WHERE shopid = @1 AND status = 0 AND wx = @2 AND typ = 1", p => p.Set(shopid).Set(prin.wx)))
                        {
                            m.P("检测到您当前有分配的摊点");
                            m.CHECKBOX(nameof(mycart), true, "完成同时扣减摊点里的数目");
                        }
                        else
                        {
                            m.P("确认已经出货并且结束此单吗？");
                        }
                        m._FORM();
                    }
                });
            }
            else // POST
            {
                mycart = (await ac.ReadAsync<Form>())[nameof(mycart)];
                using (var dc = ac.NewDbContext(ReadCommitted))
                {
                    if (dc.Query1("UPDATE orders SET status = " + FINISHED + ", finished = localtimestamp WHERE id = @1 AND shopid = @2 AND status = " + PAID + " RETURNING *", p => p.Set(orderid).Set(shopid)))
                    {
                        var o = dc.ToObject<Order>();
                        if (mycart) // deduce my cart loads
                        {
                            dc.Query1("SELECT id, items FROM orders WHERE wx = @1 AND status = 0 AND shopid = @2 AND typ = 1", p => p.Set(prin.wx).Set(shopid));
                            dc.Let(out int cartid).Let(out OrderItem[] cart);
                            if (Deduce(cart, o.items))
                            {
                                dc.Execute("UPDATE orders SET items = @1 WHERE id = @2 AND status = 0 AND shopid = @3", p => p.Set(cart).Set(cartid).Set(shopid));
                            }
                            else
                            {
                                dc.Rollback();
                                ac.GivePane(200, m => { m.CALLOUT("摊点上的数目不够扣减"); });
                                return;
                            }
                        }
                    }
                }
                ac.GivePane(200);
            }
        }
    }

    public class OprPastlyVarWork : OrderVarWork
    {
        public OprPastlyVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class AdmKickVarWork : OrderVarWork
    {
        public AdmKickVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}