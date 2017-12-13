using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Samp.Order;
using static Greatbone.Samp.User;

namespace Greatbone.Samp
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
                dc.Query("SELECT * FROM orders WHERE wx = @1 AND status = 0 ORDER BY id DESC", p => p.Set(wx));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("网点: ").T(o.shopname)._CAPTION();
                    h.BOX_().P_("收货").T(o.name)._T(o.addr)._T(o.tel).SP().TOOL("addr")._P()._BOX();
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var oi = o.items[i];
                        h.ICON("/shop/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                        h.BOX_(0x46).P(oi.name).P(oi.price)._BOX();
                        h.BOX_(0x42).P(oi.qty, null, oi.unit).TOOL("item", i)._BOX();
                        h.BOX_(0x42);
                        if (o.pos) h.P(oi.load);
                        h._BOX();
                    }
                    h.FIELD_(box: 8).T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元")._FIELD();
                    h.FIELD(o.total, "总计", box: 4);
                    h.TAIL(o.Err(), false);
                }, false, 3, "购物车");
            }
        }

        [Ui("清空"), Tool(ButtonConfirm)]
        public void clear(ActionContext ac)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE wx = @1 AND status = 0", p => p.Set(wx));
            }
            ac.GiveRedirect();
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
                dc.Query(p => p.Set(wx));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("网点: ").T(o.shopname).SEP().T("单号: ").T(o.id)._CAPTION(Statuses[o.status], o.status == PAID);
                    h.FIELD_("收货");
                    if (o.name != null) h._T(o.name);
                    if (o.addr != null) h._T(o.addr);
                    h._FIELD();
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var oi = o.items[i];
                        h.ICON("/shop/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                        h.BOX_(0x46).P(oi.name).P(oi.price)._BOX();
                        h.BOX_(0x42).P(oi.qty, null, oi.unit)._BOX();
                        h.BOX_(0x42);
                        if (o.pos) h.P(oi.load);
                        h._BOX();
                    }
                    h.FIELD_(box: 8).T(o.min).T("元起送，满").T(o.notch).T("元减").T(o.off).T("元")._FIELD();
                    h.FIELD(o.total, "总计", box: 4);
                    h.TAIL();
                }, false, 3, "我的订单");
            }
        }
    }

    [Ui("售点")]
    public class OprCartWork : OrderWork<OprCartVarWork>
    {
        public OprCartWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status = 0 AND shopid = @1 AND pos", p => p.Set(shopid));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("单号: ").T(o.id).SEP().T(o.addr)._CAPTION(o.name);
                    if (o.items != null)
                    {
                        for (int j = 0; j < o.items.Length; j++)
                        {
                            var oi = o.items[j];
                            h.FIELD(oi.name, box: 4).FIELD(oi.price, box: 4).FIELD(oi.qty, null, oi.unit, box: 4);
                        }
                    }
                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonConfirm), User(OPRMEM)]
        public void @new(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query1("SELECT name, min, notch, off FROM shops WHERE id = @1", p => p.Set(shopid));
                dc.Let(out string shopname).Let(out short min).Let(out short notch).Let(out short off);
                var o = new Order
                {
                    rev = 1,
                    status = 0,
                    shopid = shopid,
                    shopname = shopname,
                    pos = true,
                    min = min,
                    notch = notch,
                    off = off,
                    created = DateTime.Now
                };
                const short proj = -1 ^ KEY ^ Order.LATER;
                dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                dc.Execute(p => o.Write(p, proj), false);
            }
            ac.GiveRedirect();
        }

        [Ui("删除"), Tool(ButtonConfirmPick), User(OPRMEM)]
        public async Task del(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            int[] key = (await ac.ReadAsync<Form>())[nameof(key)];
            if (key != null)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("DELETE FROM orders WHERE shopid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(shopid), false);
                }
            }
            ac.GiveRedirect();
        }
    }

    [Ui("新单"), User(OPRMEM)]
    public class OprNewWork : OrderWork<OprNewVarWork>
    {
        public OprNewWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("分区域..."), Tool(AnchorPrompt)]
        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            bool inner = ac.Query[nameof(inner)];
            string filter = (string) ac.Query[nameof(filter)] ?? string.Empty;
            if (inner)
            {
                ac.GivePane(200, m =>
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Query1("SELECT areas FROM shops WHERE id = @1", p => p.Set(shopid));
                        dc.Let(out string[] areas);
                        m.FORM_();
                        m.RADIOSET<string>(nameof(filter), filter, areas);
                        m._FORM();
                    }
                });
                return;
            }

            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status = 1 AND addr LIKE @2 ORDER BY id DESC LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(filter + "%").Set(page * 20));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION();
                    h.FIELD_("收货").T(o.name)._T(o.addr)._FIELD();
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var oi = o.items[i];
                        h.FIELD(oi.name, box: 4).FIELD(oi.price, box: 4).FIELD(oi.qty, null, oi.unit, box: 4);
                    }
                    h.FIELD_(box: 8)._FIELD().FIELD(o.total, "总计", box: 4);
                    h.TAIL(o.Err(), false);
                }, false, 3);
            }
        }
    }

    [Ui("旧单"), User(OPR)]
    public class OprOldWork : OrderWork<OprOldVarWork>
    {
        public OprOldWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE shopid = @1 AND status > " + ABORTED + " ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION(Statuses[o.status], false);
                    h.FIELD(o.name, "买家", box: 6).FIELD(o.tel, "电话", box: 6);
                    h.FIELD_("地址").T(o.addr)._FIELD();
                    h.FIELDSET_("商品");
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var item = o.items[i];
                        h.FIELD(item.name, box: 6).FIELD(item.price, box: 3).FIELD(item.qty, suffix: item.unit, box: 3);
                    }
                    h.BOX_(6)._BOX().FIELD(o.total, "总价", box: 6);
                    h._FIELDSET();
                    h.TAIL();
                }, false, 3);
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

    [Ui("反馈")]
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
                dc.Query("SELECT * FROM orders WHERE status > 0 AND kick IS NOT NULL ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(page * 20));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("单号")._T(o.id).SEP().T(o.paid)._CAPTION();
                    if (o.name != null)
                    {
//                        h.FIELD(o.name, "姓名", box: 6).FIELD(o.city, "城市", box: 6);
                    }
                    h.BOX_().T(o.tel)._T(o.addr)._BOX();
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