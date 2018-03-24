using System.Threading.Tasks;
using Greatbone;
using static System.Data.IsolationLevel;
using static Greatbone.Modal;
using static Core.Order;
using static Core.CoreUtility;
using static Core.User;

namespace Core
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

        [Ui("付款", flag: 1), Tool(ButtonScript), Order('P')]
        public async Task prepay(WebContext wc)
        {
            string wx = wc[-2];
            int orderid = wc[this];
            short rev;
            decimal total;
            User prin = (User)wc.Principal;
            using (var dc = NewDbContext())
            {
                dc.Query1("SELECT rev, total, typ, name, city, addr, tel FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                dc.Let(out rev).Let(out total).Let(out short typ).Let(out string name).Let(out string city).Let(out string addr).Let(out string tel);
                if (typ == 0 && (prin.name != name || prin.city != null || prin.addr != addr || prin.tel != tel)) // normal order then save user info
                {
                    if (dc.Execute("INSERT INTO users (wx, name, city, addr, tel) VALUES (@1, @2, @3, @4, @5) ON CONFLICT (wx) DO UPDATE SET name = @2, city = @3, addr = @4, tel = @5", p => p.Set(wx).Set(prin.name = name).Set(prin.city = city).Set(prin.addr = addr).Set(prin.tel = tel)) > 0)
                    {
                        wc.SetTokenCookie(prin, 0xff ^ CREDENTIAL); // refresh client token thru cookie
                    }
                }
            }
            var (prepay_id, _) = await WeiXinUtility.PostUnifiedOrderAsync(orderid + "-" + rev, total, wx, wc.RemoteAddr.ToString(), NETADDR + "/org/paynotify", "粗粮达人-健康产品");
            if (prepay_id != null)
            {
                wc.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
            }
            else
            {
                wc.Give(500);
            }
        }

        [Ui("修改", flag: 8), Tool(ButtonShow)]
        public async Task edit(WebContext wc, int idx)
        {
            int orderid = wc[this];
            string wx = wc[-2];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    var oi = o.items[idx];
                    dc.Query1("SELECT step, stock FROM items WHERE orgid = @1 AND name = @2", p => p.Set(o.orgid).Set(oi.name));
                    dc.Let(out short step).Let(out short stock);
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("购买数量");
                        h.ICON("/org/" + o.orgid + "/" + oi.name + "/icon", width: 1);
                        h.FIELD_(width: 2)._FIELD();
                        h.NUMBER(nameof(oi.qty), oi.qty, max: stock, min: (short)0, step: step, width: 2);
                        h.FIELD(oi.unit, width: 1);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                short qty = f[nameof(qty)];
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    o.UpdItem(idx, qty);
                    o.TotalUp();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                wc.GivePane(200);
            }
        }
    }

    public class OprPosVarWork : OrderVarWork
    {
        public OprPosVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("加货"), Tool(ButtonShow), User(OPRSTAFF)]
        public async Task add(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = NewDbContext())
                    {
                        dc.Query("SELECT name, unit, price, stock FROM items WHERE orgid = @1 AND status > 0 AND stock > 0", p => p.Set(orgid));
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out string unit).Let(out decimal price).Let(out short stock);
                            h.FIELD(name, width: 5).FIELD(stock, width: 0x22).NUMBER(name + '~' + unit + '~' + price, (short)0, max: stock, min: (short)0, step: (short)1, width: 5);
                        }
                    }
                    h._FORM();
                });
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                using (var dc = NewDbContext(ReadUncommitted))
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
                wc.GivePane(200);
            }
        }

        [Ui("分派"), Tool(ButtonShow), User(OPRSTAFF)]
        public async Task assign(WebContext wc)
        {
            int orderid = wc[this];
            string orgid = wc[-2];
            string opr;
            string addr = null;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = NewDbContext())
                    {
                        // select operator info
                        dc.Query("SELECT wx, name, tel FROM users WHERE oprat = @1", p => p.Set(orgid));
                        h.SELECT_(nameof(opr), "人员");
                        while (dc.Next())
                        {
                            dc.Let(out string wx).Let(out string name).Let(out string tel);
                            h.OPTION(wx + '~' + name + '~' + tel, name);
                        }
                        h._SELECT();
                        // input addr
                        var org = Obtain<Map<string, Org>>()[orgid];
                        h.TEXT(nameof(addr), addr, "区域");
                    }
                    h._FORM();
                });
            }
            else
            {
                (await wc.ReadAsync<Form>()).Let(out opr).Let(out addr);
                var (wx, name, tel) = opr.ToTriple('~');
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orders SET wx = @1, name = @2, tel = @3, addr = @4 WHERE id = @5 AND orgid = @6",
                        p => p.Set(wx).Set(name).Set(tel).Set(addr).Set(orderid).Set(orgid));
                }
                wc.GivePane(200);
            }
        }
    }

    public class OprNewoVarWork : OrderVarWork
    {
        public OprNewoVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("撤消", "【警告】确认要撤销此单吗？实收金额将退回给买家"), Tool(ButtonConfirm)]
        public async Task abort(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            short rev = 0;
            decimal total = 0, cash = 0;
            using (var dc = NewDbContext())
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
                    using (var dc = NewDbContext(ReadUncommitted))
                    {
                        dc.Query1("UPDATE orders SET status = " + ABORTED + ", aborted = localtimestamp WHERE id = @1 AND orgid = @2 RETURNING items", p => p.Set(orderid).Set(orgid));
                        dc.Let(out OrderItem[] items);
                        for (int i = 0; i < items?.Length; i++) // revert stock
                        {
                            var oi = items[i];
                            dc.Execute("UPDATE items SET stock = stock + @1 WHERE orgid = @2 AND name = @3", p => p.Set(oi.qty).Set(orgid).Set(oi.name));
                        }
                    }
                }
            }
            wc.GiveRedirect("../");
        }

        [Ui("完成"), Tool(ButtonShow)]
        public async Task deliver(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            User prin = (User)wc.Principal;
            bool mycart;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    // check personal pos
                    using (var dc = NewDbContext())
                    {
                        h.FORM_();
                        if (dc.Query1("SELECT TRUE FROM orders WHERE orgid = @1 AND status = 0 AND wx = @2 AND typ = 1", p => p.Set(orgid).Set(prin.wx)))
                        {
                            h.P("检测到您当前有分配的摊点");
                            h.CHECKBOX(nameof(mycart), true, "完成同时扣减摊点里的数目");
                        }
                        else
                        {
                            h.P("确认已经出货并且结束此单吗？");
                        }
                        h._FORM();
                    }
                });
            }
            else // POST
            {
                mycart = (await wc.ReadAsync<Form>())[nameof(mycart)];
                using (var dc = NewDbContext(ReadCommitted))
                {
                    if (dc.Query1("UPDATE orders SET status = " + ENDED + ", closed = localtimestamp WHERE id = @1 AND orgid = @2 AND status = " + PAID + " RETURNING *", p => p.Set(orderid).Set(orgid)))
                    {
                        var o = dc.ToObject<Order>();
                        if (mycart) // deduce my cart loads
                        {
                            dc.Query1("SELECT id, items FROM orders WHERE wx = @1 AND status = 0 AND orgid = @2 AND typ = 1", p => p.Set(prin.wx).Set(orgid));
                            dc.Let(out int cartid).Let(out OrderItem[] cart);
                            if (Deduce(cart, o.items))
                            {
                                dc.Execute("UPDATE orders SET items = @1 WHERE id = @2 AND status = 0 AND orgid = @3", p => p.Set(cart).Set(cartid).Set(orgid));
                            }
                            else
                            {
                                dc.Rollback();
                                wc.GivePane(200, m => { m.P("摊点上的数目不够扣减"); });
                                return;
                            }
                        }
                    }
                }
                wc.GivePane(200);
            }
        }
    }

    public class OprOldoVarWork : OrderVarWork
    {
        public OprOldoVarWork(WorkConfig cfg) : base(cfg)
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