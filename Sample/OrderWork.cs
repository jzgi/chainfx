using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, long>((obj) => ((Order) obj).id);
        }
    }

    [Ui("购物车")]
    public class MyCartWork : OrderWork<MyCartVarWork>
    {
        public MyCartWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty).T(" FROM orders WHERE wx = @1 AND status = 0 ORDER BY id DESC");
                if (dc.Query(p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION(o.preparing ? "备货中" : null, o.preparing);
                        h.FIELD_("收货");
                        if (o.name != null) h._T(o.name);
                        if (o.city != null) h._T(o.city);
                        if (o.area != null) h._T(o.area);
                        if (o.addr != null) h._T(o.addr);
                        h.BUTTONSHOW("填写地址", o.id + "/addr", 1, "填写收货地址");
                        h._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.IMG("/shop/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                            h.BOX_(5).P(oi.name).P(oi.price)._BOX();
                            h.BOX_(5).P(oi.qty, suffix: oi.unit).P(oi.opts).BUTTONSHOW("修改", o.id + "/item-" + i)._BOX();
                        }
                        h.BOX_(7).T("<p>").T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元").T("</p>")._BOX();
                        h.BOX_(5).P(o.total, "总计")._BOX();
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, null, false, 3);
                }
            }
        }

        /// <summary>
        /// To create a new order or add item to an existing order.
        /// </summary>
        public async Task add(ActionContext ac)
        {
            string wx = ac[-1];
            var f = await ac.ReadAsync<Form>();
            string city = f[nameof(city)];
            string area = f[nameof(area)];
            string shopid = f[nameof(shopid)];
            string shopname = f[nameof(shopname)];
            string name = f[nameof(name)];
            decimal price = f[nameof(price)];
            short qty = f[nameof(qty)];
            string unit = f[nameof(unit)];
            string[] opts = f[nameof(opts)];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT id, items, total FROM orders WHERE shopid = @1 AND wx = @2 AND status = 0", p => p.Set(shopid).Set(wx)))
                {
                    var o = new Order();
                    dc.Let(out o.id).Let(out o.items).Let(out o.total);
                    o.AddItem(name, price, qty, unit, opts);
                    o.SetTotal();
                    dc.Execute("UPDATE orders SET rev = rev + 1, items = @1, total = @2 WHERE id = @3", p => p.Set(o.items).Set(o.total).Set(o.id));
                }
                else
                {
                    User prin = (User) ac.Principal;
                    var o = new Order
                    {
                        rev = 1,
                        shopid = shopid,
                        shopname = shopname,
                        wx = prin.wx,
                        name = prin.name,
                        tel = prin.tel,
                        city = city ?? prin.city,
                        area = area ?? prin.area,
                        addr = prin.addr,
                        items = new[] {new OrderItem {name = name, price = price, qty = qty, unit = unit, opts = opts}},
                    };
                    o.SetTotal();
                    const short proj = -1 ^ Order.ID ^ Order.LATER;
                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.Give(200);
            }
        }
    }

    [Ui("我的订单")]
    public class MyOrderWork : OrderWork<MyOrderVarWork>
    {
        public MyOrderWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(Order.Empty).T(" FROM orders WHERE wx = @1 AND status > 0 ORDER BY id DESC");
                if (dc.Query(p => p.Set(wx)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION(o.preparing ? "备货中" : null, o.preparing);
                        h.FIELD_("收货");
                        if (o.name != null) h._T(o.name);
                        if (o.city != null) h._T(o.city);
                        if (o.area != null) h._T(o.area);
                        if (o.addr != null) h._T(o.addr);
                        h._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.IMG("/shop/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                            h.BOX_(5).P(oi.name).P(oi.price)._BOX();
                            h.BOX_(5).P(oi.qty, suffix: oi.unit).P(oi.opts)._BOX();
                        }
                        h.BOX_(7).T("<p>").T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元").T("</p>")._BOX();
                        h.BOX_(5).P(o.total, "总计")._BOX();
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, null, false, 3);
                }
            }
        }
    }

    [Ui("新单")]
    [User(OPRMEM)]
    public class OprNewWork : OrderWork<OprNewVarWork>
    {
        public OprNewWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.PAID + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION(o.preparing ? "备货中" : null, o.preparing);
                        h.FIELD_("收货");
                        if (o.name != null) h._T(o.name);
                        if (o.city != null) h._T(o.city);
                        if (o.area != null) h._T(o.area);
                        if (o.addr != null) h._T(o.addr);
                        h._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.IMG("/shop/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                            h.BOX_(5).P(oi.name).P(oi.price)._BOX();
                            h.BOX_(5).P(oi.qty, suffix: oi.unit).P(oi.opts)._BOX();
                        }
                        h.BOX_(7).T("<p>").T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元").T("</p>")._BOX();
                        h.BOX_(5).P(o.total, "总计")._BOX();
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
        }

        [Ui("备货状态"), Style(ButtonConfirm)]
        public async Task prepare(ActionContext ac)
        {
            string shopid = ac[-1];
            var f = await ac.ReadAsync<Form>();
            int[] key = f[nameof(key)];
            if (key != null)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("UPDATE orders SET prepare = TRUE WHERE shopid = @1 AND id")._IN_(key).T(" RETURNING wx, total");
                    if (dc.Query(p => p.Set(shopid)))
                    {
                        while (dc.Next())
                        {
                            dc.Let(out string wx).Let(out decimal total);
                            await WeiXinUtility.PostSendAsync(wx, "【通知】正在为您的订单备货生产（金额" + total + "）");
                        }
                    }
                }
            }
            ac.GiveRedirect();
        }
    }

    [Ui("派单")]
    [User(OPR)]
    public class OprGoWork : OrderWork<OprGoVarWork>
    {
        public OprGoWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = " + Order.READY + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION();
                        if (o.name != null)
                        {
                            h.FIELD(o.name, "姓名", box: 6).FIELD(o.city, "城市", box: 6);
                        }
                        h.BOX_().T(o.tel)._T(o.area)._T(o.addr)._BOX();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var item = o.items[i];
                            h.FIELD(item.name, box: 4).FIELD(item.price, box: 4).FIELD(item.qty, suffix: item.unit, box: 4);
                        }
                        h.FIELD(o.total, "总价");
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
        }

        [Ui("查询")]
        public void send(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            ac.GiveRedirect();
        }
    }

    [Ui("旧单")]
    [User(OPR)]
    public class OprPastWork : OrderWork<OprPastVarWork>
    {
        public OprPastWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status > " + Order.ABORTED + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION();
                        if (o.name != null)
                        {
                            h.FIELD(o.name, "姓名", box: 6).FIELD(o.city, "城市", box: 6);
                        }
                        h.BOX_().T(o.tel)._T(o.area)._T(o.addr)._BOX();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var item = o.items[i];
                            h.FIELD(item.name, box: 4).FIELD(item.price, box: 4).FIELD(item.qty, suffix: item.unit, box: 4);
                        }
                        h.FIELD(o.total, "总价");
                    }, false, 3);
                }
                else
                {
                    ac.GiveGridPage(200, (Order[]) null, (h, o) => { }, false, 3);
                }
            }
        }

        [Ui("查询")]
        public void send(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];

            using (var dc = ac.NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            ac.GiveRedirect();
        }
    }

    [Ui("客服")]
    [User(adm: true)]
    public class AdmKickWork : OrderWork<AdmKickVarWork>
    {
        public AdmKickWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status >  " + Order.READY + " AND kick IS NOT NULL ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(page * 20));
                ac.GiveGridPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION();
                    if (o.name != null)
                    {
                        h.FIELD(o.name, "姓名", box: 6).FIELD(o.city, "城市", box: 6);
                    }
                    h.BOX_().T(o.tel)._T(o.area)._T(o.addr)._BOX();
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var item = o.items[i];
                        h.FIELD(item.name, box: 4).FIELD(item.price, box: 4).FIELD(item.qty, suffix: item.unit, box: 4);
                    }
                    h.FIELD(o.total, "总价");
                }, false, 3);
            }
        }
    }
}