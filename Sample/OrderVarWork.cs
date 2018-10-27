using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
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

        [Ui("撤单"), Tool(AnchorShow), OrderState('C')]
        public async Task cancel(WebContext wc, int idx)
        {
            int orderid = wc[this];
            int myid = wc[-2];
            if (wc.IsGet)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND custid = @2", p => p.Set(orderid).Set(myid));
                    var item = Obtain<Map<string, Item>>()[o.item];
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDUL_("购买数量");
                        h.LI_().LABEL("货品").PIC("/" + o.item + "/icon", css: "uk-width-1-6").SP().T(o.item)._LI();
                        //                        h.NUMBER(nameof(oi.qty), oi.qty, "购量", max: item.max, min: (short) 0, step: item.step);
                        h._FIELDUL();
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
                    var o = dc.Query1<Order>("SELECT * FROM orders WHERE id = @1 AND custid = @2", p => p.Set(orderid).Set(myid));
                    //                    o.UpdItem(idx, qty);
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2, net = @3 WHERE id = @4", p => p.Set(o.item).Set(o.total).Set(o.id));
                }
                wc.GivePane(200);
            }
        }
    }

    public class HubOrderVarWork : OrderVarWork
    {
        public HubOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui(icon: "list", tip: "团组排队订单", group: 0b000011), Tool(ButtonOpen)]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM orders WHERE hubid = @1 AND status = ").T(Order.PAID).T(" AND teamid = @2 ORDER BY id ASC");
                var arr = dc.Query<Order>(p => p.Set(hubid).Set(teamid));
                wc.GivePane(200, h => { h.BOARD(arr, o => { h.T(o.item); }); });
            }
        }

        [Ui(icon: "list", tip: "工坊备货订单", group: 0b000101), Tool(ButtonOpen)]
        public void accepted(WebContext wc)
        {
            string hubid = wc[0];
            short shopid = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM orders WHERE hubid = @1 AND status = ").T(Order.CONFIRMED).T(" AND shopid = @2 ORDER BY id ASC");
                var arr = dc.Query<Order>(p => p.Set(hubid).Set(shopid));
                wc.GivePane(200, h => { h.BOARD(arr, o => { h.T(o.item); }); });
            }
        }

        [Ui("接货", tip: "中转接货", group: 0b000100), Tool(ButtonShow)]
        public async Task stock(WebContext wc)
        {
            string hubid = wc[0];
            short shopid = wc[this];
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT id,  itemid, item, qty FROM orders WHERE hubid = @1 AND status = ").T(Order.CONFIRMED).T(" AND shopid = @2 ORDER BY itemid");
                        dc.Query(p => p.Set(hubid).Set(shopid));
                        h.FORM_();
                        h.ORDERSCALE(dc);
                        h._FORM();
                        h.T("<script>initform();</script>");
                    }
                });
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                int[] id = f[nameof(id)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET stockerid = @1, stocker = @2, status = ").T(Order.ACCEPTED).T(" WHERE hubid = @3 AND status = ").T(Order.CONFIRMED).T(" AND shopid = @4 AND ")._IN_(id);
                    dc.Execute(prepare: false);
                }
                wc.GivePane(200);
            }
        }

        [Ui("倒退", tip: "倒退给备货", group: 0b001000), Tool(ButtonOpen)]
        public void unstock(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
        }

        [Ui(icon: "list", tip: "团组订单", group: 0b001001), Tool(ButtonOpen)]
        public void stocked(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM orders WHERE hubid = @1 AND status = ").T(Order.ACCEPTED).T(" AND teamid = @2 ORDER BY id ASC");
                var arr = dc.Query<Order>(p => p.Set(hubid).Set(teamid));
                wc.GivePane(200, h => { h.BOARD(arr, o => { h.T(o.item); }); });
            }
        }

        [Ui("派运", tip: "团组订单", group: 0b001000), Tool(ButtonOpen)]
        public void send(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT id,  itemid, item, qty FROM orders WHERE hubid = @1 AND status = ").T(Order.ACCEPTED).T(" AND teamid = @2 ORDER BY itemid");
                        dc.Query(p => p.Set(hubid).Set(teamid));
                        h.FORM_();
                        h.ORDERSCALE(dc);
                        h._FORM();
                        h.T("<script>initform();</script>");
                    }
                });
            }
            else
            {
            }
        }

        [Ui(icon: "list", tip: "团组订单", group: 0b010001), Tool(ButtonOpen)]
        public void sent(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM orders WHERE hubid = @1 AND status = ").T(Order.SENT).T(" AND teamid = @2 ORDER BY id ASC");
                var arr = dc.Query<Order>(p => p.Set(hubid).Set(teamid));
                wc.GivePane(200, h => { h.BOARD(arr, o => { h.T(o.item); }); });
            }
        }

        [Ui("倒退", tip: "倒退给中转", group: 0b010000), Tool(ButtonOpen)]
        public void unsend(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
        }

        [Ui(icon: "list", tip: "团组订单", group: 0b100001), Tool(ButtonOpen)]
        public void received(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT * FROM orders WHERE hubid = @1 AND status = ").T(Order.RECEIVED).T(" AND teamid = @2 ORDER BY id ASC");
                var arr = dc.Query<Order>(p => p.Set(hubid).Set(teamid));
                wc.GivePane(200, h => { h.BOARD(arr, o => { h.T(o.item); }); });
            }
        }


        [Ui("撤消", tip: "确认要撤销此单吗？实收款项将退回给买家", group: 0b10000000), Tool(ButtonPickConfirm)]
        public async Task abort(WebContext wc)
        {
            string orgid = wc[-2];
            int orderid = wc[this];
            short rev = 0;
            decimal cash = 0;
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT rev, cash FROM orders WHERE id = @1 AND status = 1", p => p.Set(orderid)))
                {
                    dc.Let(out rev).Let(out cash);
                }
            }
            if (cash > 0)
            {
                string hubid = wc[0];
                var hub = Obtain<Map<string, Hub>>()[hubid];
                string err = await hub.PostRefundAsync(orderid + "-" + rev, cash, cash);
                if (err == null) // success
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE orders SET status = -1, aborted = localtimestamp WHERE id = @1 AND orgid = @2", p => p.Set(orderid).Set(orgid));
                    }
                }
            }
            wc.GiveRedirect("../");
        }
    }

    public class TeamOrderVarWork : OrderVarWork
    {
        public TeamOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("收货"), Tool(ButtonPickPrompt)]
        public async Task receive(WebContext wc)
        {
            string grpid = wc[-1];
            if (wc.IsGet)
            {
                int[] key = wc.Query[nameof(key)];
                wc.GivePane(200, h =>
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT item, sum(qty) AS num FROM orders WHERE id")._IN_(key).T(" AND status = 3 AND teamid = @1 GROUP BY item");
                        dc.Query(p => p.Set(grpid));
                        h.FORM_();

                        h.T("仅列出已送达货品");
                        h.T("<table class=\"uk-table\">");
                        while (dc.Next())
                        {
                            dc.Let(out string item).Let(out short num);
                            h.TD(item).TD(num);
                        }
                        h.T("</table>");
                        h.CHECKBOX("", false, "我确认收货", required: true);
                        h._FORM();
                    }
                });
            }
            else // POST
            {
                int[] key = (await wc.ReadAsync<Form>())[nameof(key)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = 4 WHERE id")._IN_(key).T(" AND status = 3 AND grpid = @1");
                    dc.Execute(p => p.Set(grpid));
                }
                wc.GiveRedirect();
            }
        }
    }

    public class ShopOrderVarWork : OrderVarWork
    {
        public ShopOrderVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}