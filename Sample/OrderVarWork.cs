using System.Threading.Tasks;
using Greatbone.Core;
using static System.Data.IsolationLevel;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.Order;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
{
    public abstract class OrderVarWork : Work
    {
        protected OrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MyCartVarWork : OrderVarWork
    {
        public MyCartVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改", Group = -1), Tool(ButtonShow)]
        public async Task addr(ActionContext ac)
        {
            int orderid = ac[this];
            string wx = ac[-2];
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query1("SELECT shopid, pos, addr, tel FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                        dc.Let(out string oshopid).Let(out bool opos).Let(out string oaddr).Let(out string otel);
                        dc.Query1("SELECT city, areas FROM shops WHERE id = @1", p => p.Set(oshopid));
                        dc.Let(out string city).Let(out string[] areas);
                        h.FIELDSET_("收货地址");
                        string tel;
                        if (areas != null) // limited delivery areas
                        {
                            ac.Query.Let(out string a).Let(out string b).Let(out string c).Let(out tel); // by select refresh
                            if (a == null) // init from order
                            {
                                (a, b, c) = oaddr.To3Strings('\a');
                                a = City.ResolveIn(a, areas);
                                tel = otel;
                            }
                            var sites = City.SitesOf(city, a);
                            b = City.ResolveIn(b, sites);
                            h.SELECT(nameof(a), a, areas, refresh: true, box: 4).SELECT(nameof(b), b, sites, box: 4).TEXT(nameof(c), c, box: 4);
                        }
                        else // formless address
                        {
                            ac.Query.Let(out string a).Let(out tel);
                            if (a == null)
                            {
                                a = oaddr;
                                tel = otel;
                            }
                            h.TEXT(nameof(a), a, tip: "填写您的完整地址");
                        }
                        h.TEL(nameof(tel), tel, "您的随身电话", required: true);
                    }
                    h._FORM();
                });
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                string a = f[nameof(a)];
                string b = f[nameof(b)];
                string c = f[nameof(c)];
                string tel = f[nameof(tel)];
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT pos FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    dc.Let(out bool pos);
                    string addr = pos ? a + '\a' + b + '\a' + c : a;
                    dc.Execute("UPDATE orders SET addr = @1, tel = @2 WHERE id = @3", p => p.Set(addr).Set(tel).Set(orderid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("修改", Group = -1), Tool(ButtonShow)]
        public async Task item(ActionContext ac, int idx)
        {
            int orderid = ac[this];
            string wx = ac[-2];
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query1("SELECT * FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    var o = dc.ToObject<Order>();
                    var oi = o.items[idx];
                    dc.Query1("SELECT step, stock FROM items WHERE shopid = @1 AND name = @2", p => p.Set(o.shopid).Set(oi.name));
                    dc.Let(out short step).Let(out short stock);
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("修改数量");
                        h.ICON("/shop/" + o.shopid + "/" + oi.name + "/icon", box: 2);
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
                    dc.Query1("SELECT * FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                    var o = dc.ToObject<Order>();
                    o.UpdItem(idx, qty);
                    o.TotalUp();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                ac.GivePane(200);
            }
        }

        [Ui("付款"), Tool(ButtonScript), Order('P')]
        public async Task Prepay(ActionContext ac)
        {
            string wx = ac[-2];
            int orderid = ac[this];
            short rev;

            decimal total;
            using (var dc = ac.NewDbContext())
            {
                dc.Query1("SELECT rev, total FROM orders WHERE id = @1 AND wx = @2", p => p.Set(orderid).Set(wx));
                dc.Let(out rev).Let(out total);
            }
            var (prepay_id, _) = await WeiXinUtility.PostUnifiedOrderAsync(orderid + "-" + rev, total, wx, ac.RemoteAddr, "http://144000.tv/paynotify", "粗粮达人-健康产品");
            if (prepay_id != null)
            {
                ac.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
            }
            else
            {
                ac.Give(500);
            }
        }
    }

    public class MyOrderVarWork : OrderVarWork
    {
        public MyOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("建议"), Tool(ButtonShow)]
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

        [Ui("加货"), Tool(ButtonShow, 2), User(OPRMEM)]
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
                            m.FIELD(name, box: 5).FIELD(stock, sign: unit, box: 0x22).NUMBER(name + '~' + unit + '~' + price, (short) 0, min: (short) 0, step: (short) 1, max: stock, box: 5);
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
                    dc.Query1("SELECT * FROM orders WHERE id = @1", p => p.Set(orderid));
                    var o = dc.ToObject<Order>();
                    for (int i = 0; i < f.Count; i++)
                    {
                        var e = f.At(i);
                        var (name, unit, price) = e.Key.To3Strings('~');
                        short n = e.Value;
                        if (n != 0) o.ReceiveItem(name, unit, decimal.Parse(price), n);
                    }
                    dc.Execute("UPDATE orders SET items = @1 WHERE id = @2", p => p.Set(o.items).Set(o.id));
                }
                ac.GivePane(200);
            }
        }

        [Ui("分派"), Tool(ButtonShow), User(OPRMEM)]
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
                        dc.Query1("SELECT areas FROM shops WHERE id = @1", p => p.Set(shopid));
                        dc.Let(out string[] areas);
                        if (areas != null)
                        {
                            m.SELECT(nameof(addr), addr, areas, "区域");
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
                var (wx, name, tel) = opr.To3Strings('~');
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE orders SET wx = @1, name = @2, tel = @3, addr = @4 WHERE id = @5 AND shopid = @6",
                        p => p.Set(wx).Set(name).Set(tel).Set(addr).Set(orderid).Set(shopid));
                }
                ac.GivePane(200);
            }
        }
    }

    public class OprNewieVarWork : OrderVarWork
    {
        public OprNewieVarWork(WorkConfig cfg) : base(cfg)
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

        [Ui("完成", "确认完成？"), Tool(ButtonConfirm)]
        public void finish(ActionContext ac)
        {
            string shopid = ac[-2];
            int orderid = ac[this];
            using (var dc = ac.NewDbContext(ReadCommitted))
            {
                if (dc.Query1("UPDATE orders SET status = @1 WHERE id = @2 AND shopid = @3 AND status = " + PAID + "RETURNING pos, wx, name, addr, tel", p => p.Set(FINISHED).Set(orderid).Set(shopid)))
                {
                    dc.Let(out bool pos).Let(out string wx).Let(out string name).Let(out string addr).Let(out string tel);
                    if (!pos) // if ordinary order then update user info
                    {
                        dc.Execute("UPDATE users SET name = COALESCE(@1, name), addr = COALESCE(@2, addr), tel = COALESCE(@3, tel) WHERE wx = @4", p => p.Set(name).Set(addr).Set(tel).Set(wx));
                    }
                }
            }
            ac.GiveRedirect("../");
        }
    }

    public class OprOldieVarWork : OrderVarWork
    {
        public OprOldieVarWork(WorkConfig cfg) : base(cfg)
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