using System.Data;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.Order;

namespace Greatbone.Samp
{
    public class OrderlyAttribute : StateAttribute
    {
        readonly char state;

        public OrderlyAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(object obj)
        {
            var o = obj as Order;
            if (state == 'A')
                return o.addr != null;
            return false;
        }
    }

    public abstract class OrderVarWork : Work
    {
        protected OrderVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class MyCartVarWork : OrderVarWork
    {
        public MyCartVarWork(WorkContext wc) : base(wc)
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
                    using (var dc = Service.NewDbContext())
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

        [Ui("付款"), Tool(ButtonScript), Orderly('A')]
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
            var (prepay_id, _) = await WeiXinUtility.PostUnifiedOrderAsync(orderid + "-" + rev, total, wx, ac.RemoteAddr, "http://144000.tv/paynotify");
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
        public MyOrderVarWork(WorkContext wc) : base(wc)
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
        public OprCartVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("加货"), Tool(ButtonShow, 2)]
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
                            m.FIELD(name, box: 5).FIELD(stock, box: 0x22).NUMBER(name + '~' + unit + '~' + price, (short) 0, min: (short) 0, step: (short) 1, max: stock, box: 5);
                        }
                    }
                    m._FORM();
                });
            }
            else // POST
            {
                var f = await ac.ReadAsync<Form>();
                using (var dc = ac.NewDbContext(IsolationLevel.ReadCommitted))
                {
                    dc.Query1("SELECT * FROM orders WHERE id = @1", p => p.Set(orderid));
                    var o = dc.ToObject<Order>();
                    for (int i = 0; i < f.Count; i++)
                    {
                        var e = f.At(i);
                        var (name, unit, price) = e.Key.To3Strings('~');
                        short pr = e.Value;
                        o.ReceiveItem(name, unit, decimal.Parse(price), pr);
                        dc.Execute("UPDATE items SET stock = stock - @1 WHERE shopid = @2 AND name = @3", p => p.Set(pr).Set(shopid).Set(name));
                    }
                    dc.Execute("UPDATE orders SET items = @1 WHERE id = @2", p => p.Set(o.items).Set(o.id));
                }
                ac.GivePane(200);
            }
        }

        [Ui("分派"), Tool(ButtonShow)]
        public async Task assign(ActionContext ac)
        {
            string shopid = ac[-2];
            int orderid = ac[this];
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    string opr = null;
                    string addr = null;
                    // operator list
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query("SELECT wx, name, tel FROM users WHERE oprat = @1", p => p.Set(shopid));
                        m.SELECT_(nameof(opr), "人员");
                        while (dc.Next())
                        {
                            dc.Let(out string wx).Let(out string name).Let(out string tel);
                            m.OPTION(wx, name);
                        }
                        m._SELECT();

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
            }
        }
    }

    public class OprNewVarWork : OrderVarWork
    {
        public OprNewVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("撤单"), Tool(ButtonShow)]
        public async Task abort(ActionContext ac)
        {
            int orderid = ac[this];
            if (ac.GET)
            {
                ac.GivePane(200, m => { m.FORM_().CALLOUT("确定要撤销此单，实收金额退回给买家?")._FORM(); });
            }
            else
            {
                decimal total = 0, cash = 0;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT total, cash FROM orders WHERE id = @1", p => p.Set(orderid)))
                    {
                        dc.Let(out total).Let(out cash);
                    }
                }
                string err = await WeiXinUtility.PostRefundAsync(orderid, total, cash);
                if (err == null) // success
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(ABORTED).Set(orderid));
                    }
                    ac.GivePane(200);
                }
                else // err
                {
                    ac.GivePane(200, m => { m.FORM_().CALLOUT(err).CALLOUT("确定重复操作吗？")._FORM(); });
                }
            }
        }

        [Ui("出单"), Tool(ButtonShow)]
        public async Task deduct(ActionContext ac)
        {
            int orderid = ac[this];
            if (ac.GET)
            {
                ac.GivePane(200, m => { m.FORM_().CALLOUT("确定要撤销此单，实收金额退回给买家?")._FORM(); });
            }
            else
            {
                decimal total = 0, cash = 0;
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT total, cash FROM orders WHERE id = @1", p => p.Set(orderid)))
                    {
                        dc.Let(out total).Let(out cash);
                    }
                }
                string err = await WeiXinUtility.PostRefundAsync(orderid, total, cash);
                if (err == null) // success
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = @1 WHERE id = @2", p => p.Set(ABORTED).Set(orderid));
                    }
                    ac.GivePane(200);
                }
                else // err
                {
                    ac.GivePane(200, m => { m.FORM_().CALLOUT(err).CALLOUT("确定重复操作吗？")._FORM(); });
                }
            }
        }
    }

    public class OprOldVarWork : OrderVarWork
    {
        public OprOldVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("退款核查", "实时核查退款到账情况"), Tool(AnchorOpen)]
        public async Task refundq(ActionContext ac)
        {
            int orderid = ac[this];

            string err = await WeiXinUtility.PostRefundQueryAsync(orderid);
            if (err == null) // success
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.CALLOUT("退款成功", false);
                    m._FORM();
                });
            }
            else
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.CALLOUT(err);
                    m.CHECKBOX("ok", false, "重新提交退款请求", required: true);
                    m.BUTTON("", 1, "确认");
                    m._FORM();
                });
            }
        }
    }

    public class AdmKickVarWork : OrderVarWork
    {
        public AdmKickVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}