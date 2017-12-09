using System;
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

        [Ui("地址", Group = -1), Tool(ButtonShow, 1)]
        public async Task addr(ActionContext ac)
        {
            string wx = ac[-2];
            int orderid = ac[this];
            string name = null;
            string area = null;
            string addr = null;
            string tel = null;
            User prin = (User) ac.Principal;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    using (var dc = Service.NewDbContext())
                    {
                        if (dc.Query1("SELECT city, areas FROM shops WHERE id = (SELECT shopid FROM orders WHERE id = @1)", p => p.Set(orderid)))
                        {
                            dc.Let(out string city).Let(out string[] areas);
                            h.FORM_();
                            if (areas == null)
                            {
                                h.TEXT(nameof(name), prin.name, "姓名", required: true);
//                                h.SELECT(nameof(area), prin.area, City.FindCity(city).Areas, "区域", required: true);
                                h.TEXT(nameof(addr), addr, "地址");
                            }
                            else
                            {
//                                h.SELECT(nameof(area), prin.area, areas, "限送", refresh: true, required: true);
//                                var places = City.All[city].FindArea(areas[0]).places;
//                                h.SELECT(nameof(addr), places[0], places, "地址", box: 7).TEXT(nameof(addr), addr, box: 5);
                            }
                            h.TEL(nameof(tel), tel, "电话", required: true);
                            h._FORM();
                        }
                    }
                });
                return;
            }

            var f = await ac.ReadAsync<Form>();
            f.Let(out area).Let(out addr).Let(out tel);
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("UPDATE orders SET area = @1, addr = @2, tel = @3 WHERE id = @4", p => p.Set(area).Set(addr).Set(tel).Set(orderid));

                if (prin.city == null)
                {
                    dc.Execute("INSERT INTO users (wx, name, city, area, addr, tel, created) VALUES (@1, @2, @3, @4, @5, @6, @7) ON CONFLICT (wx) DO UPDATE SET name = @2, city = @3, distr = @4, addr = @5, tel = @6, created = @7", p => p.Set(wx).Set(area).Set(addr).Set(tel).Set(DateTime.Now));
                    prin.city = area;
                    prin.addr = addr;
                    prin.tel = tel;
                    // refresh the client cookie
                    ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
                }
            }
            ac.GivePane(200);
        }

        [Ui("修改", Group = -1), Tool(ButtonShow, 1)]
        public async Task item(ActionContext ac, int idx)
        {
            string wx = ac[-2];
            int orderid = ac[this];

            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("SELECT ").columnlst(Empty).T(" FROM orders WHERE id = @1 AND wx = @2");
                    dc.Query1(p => p.Set(orderid).Set(wx));
                    var order = dc.ToObject<Order>();
                    var o = order.items[idx];

                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("数量");
                        h.HIDDEN(nameof(o.unit), o.unit);
                        h.HIDDEN(nameof(o.price), o.price);
                        h.NUMBER(nameof(o.qty), o.qty, min: (short) 0, max: (short) 20, step: (short) 1);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
                return;
            }

            var f = await ac.ReadAsync<Form>();
            short qty = f[nameof(qty)];
            string[] opts = f[nameof(opts)];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Empty).T(" FROM orders WHERE id = @1 AND wx = @2");
                dc.Query1(p => p.Set(orderid).Set(wx));
                var o = dc.ToObject<Order>();
                o.UpdItem(o.name, qty);
                o.SetTotal();
                dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
            }
            ac.GivePane(200);
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
                    string opr = null;
                    string addr = null;
                    // operator list
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query("SELECT name, max FROM items WHERE shopid = @1 AND status > 0 AND max > 0", p => p.Set(shopid));
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out short max);
                            m.FIELD(name, box: 7).NUMBER(name, (short) 0, min: (short) 0, step: (short) 1, max: max, box: 5);
                        }
                    }

                    m._FORM();
                });
                return;
            }

            var f = await ac.ReadAsync<Form>();
            using (var dc = ac.NewDbContext())
            {
                dc.Query1("SELECT * FROM orders WHERE id = @1", p => p.Set(orderid));
                var o = dc.ToObject<Order>();
                dc.Let(out Item[] items);
                for (int i = 0; i < f.Count; i++)
                {
                    var e = f.At(i);
//                    o.AddItem(e.key, );
                }
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