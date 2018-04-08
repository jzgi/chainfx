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

        [Ui("付款"), Tool(ButtonScript), Order('P')]
        public async Task prepay(WebContext wc)
        {
            string wx = wc[-2];
            int orderid = wc[this];
            Order o = null;
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ Order.DETAIL;
                dc.Sql("SELECT ").collst(Order.Empty, proj).T(" FROM orders WHERE id = @1 AND custwx = @2");
                o = dc.Query1<Order>(p => p.Set(orderid).Set(wx), proj);
            }
            var (prepay_id, _) = await WeiXinUtility.PostUnifiedOrderAsync(
                orderid + "-" + o.rev,
                (o.comp ? o.net : o.total),
                wx,
                wc.RemoteAddr.ToString(),
                NETADDR + "/org/paynotify",
                "粗粮达人-健康产品"
            );
            if (prepay_id != null)
            {
                wc.Give(200, WeiXinUtility.BuildPrepayContent(prepay_id));
            }
            else
            {
                wc.Give(500);
            }
        }

        [Ui("修改"), Tool(ButtonShow, Style.None, 1)]
        public async Task Upd(WebContext wc, int idx)
        {
            int orderid = wc[this];
            string wx = wc[-2];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND custwx = @2", p => p.Set(orderid).Set(wx));
                    var it = o.items[idx];
                    dc.Query1("SELECT step, stock FROM items WHERE orgid = @1 AND name = @2", p => p.Set(o.orgid).Set(it.name));
                    dc.Let(out short step).Let(out short stock);
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("购买数量");
                        h.FIELD_("货品").ICON("/" + o.orgid + "/" + it.name + "/icon", wid: 0x16)._T(it.name)._FIELD();
                        h.NUMBER(nameof(it.qty), it.qty, "购量", max: stock, min: (short)0, step: step);
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
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND custwx = @2", p => p.Set(orderid).Set(wx));
                    o.UpdItem(idx, qty);
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2, net = @3 WHERE id = @4", p => p.Set(o.items).Set(o.total).Set(o.net).Set(o.id));
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

        [Ui("发货"), Tool(ButtonShow, size: 1)]
        public async Task deliver(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            bool comp = false;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Query1("SELECT comp FROM orders WHERE id = @1 AND orgid = @2", p => p.Set(orderid).Set(orgid));
                    dc.Let(out comp);
                }
                wc.GivePane(200, h => { h.FORM_().FIELDSET_("是否适用佣金").CHECKBOX(nameof(comp), comp, "计算销售佣金")._FIELDSET()._FORM(); });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                comp = f[nameof(comp)];
                using (var dc = NewDbContext())
                {
                    dc.Query1("SELECT items FROM orders WHERE id = @1 AND orgid = @2", p => p.Set(orderid).Set(orgid));
                    dc.Let(out OrderItem[] items);
                    OrderItem.Ship(items);
                    dc.Execute("UPDATE orders SET items = @1, comp = @2 WHERE id = @3 AND orgid = @4", p => p.Set(items).Set(comp).Set(orderid).Set(orgid));
                }
                wc.GivePane(200);
            }
        }


        [Ui("完成"), Tool(ButtonShow)]
        public async Task end(WebContext wc)
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
                            dc.Execute("UPDATE orders SET items = @1 WHERE id = @2 AND status = 0 AND orgid = @3", p => p.Set(cart).Set(cartid).Set(orgid));
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