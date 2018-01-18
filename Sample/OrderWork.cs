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
        protected OrderWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, long>((obj) => ((Order) obj).id);
        }
    }

    [Ui("我的订单")]
    public class MyOrderWork : OrderWork<MyOrderVarWork>
    {
        public MyOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string wx = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                var arr = dc.Query<Order>(dc.Sql("SELECT ").columnlst(Order.Empty).T(" FROM orders WHERE wx = @1 ORDER BY status, id DESC"), p => p.Set(wx));
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();

                    bool bgn = false;
                    m.BOARDVIEW_();
                    foreach (var o in arr)
                    {
                        if (!bgn && o.status > 0)
                        {
                            bgn = true;
                            m.T("<h4>历史订单</h4>");
                        }
                        m.CARD_(o);
                        m.CAPTION_().T("卖方: ").T(o.shopname)._CAPTION(Statuses[o.status], o.status <= PAID);

                        m.FIELD_("收货", box: 0x4a).T(o.city).T(o.addr)._T(o.name).BR().T(o.tel)._FIELD().FIELD_(box: 2);
                        if (o.status == 0) m.TOOL("addr");
                        m._FIELD();

                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            if (o.status <= 1)
                            {
                                m.ICON("/" + o.shopid + "/" + oi.name + "/icon", box: 2);
                                m.BOX_(0x46).P(oi.name).P(oi.price, sign: "¥")._BOX();
                                m.BOX_(0x42).P(oi.qty, null, oi.unit);
                                if (o.status == 0) m.TOOL("item", i);
                                m._BOX();
                                m.BOX_(0x42);
                                if (o.typ == POS) m.P(oi.load, sign: oi.unit);
                                m._BOX();
                            }
                            else
                            {
                                m.FIELD_().T(oi.name)._T("¥").T(oi.price).T(oi.qty).T(oi.unit)._FIELD();
                            }
                        }
                        m.FIELD(o.min + "元起订，每满" + o.notch + "元立减" + o.off + "元", box: 8);
                        m.FIELD(o.total, "总计", sign: "¥", box: 4);

                        if (o.status == 0) m.TAIL(o.Err(), group: 0);
                        else m.TAIL();

                        m._CARD();
                    }
                    m._BOARDVIEW(arr?.Length ?? 0);
                }, false, 3);
            }
        }

        [Ui("清空购物车"), Tool(ButtonConfirm)]
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

    [Ui("摊点")]
    public class OprCartWork : OrderWork<OprCartVarWork>
    {
        public OprCartWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status = 0 AND shopid = @1 AND typ = 1", p => p.Set(shopid));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("#").T(o.id).SEP().T(o.addr)._CAPTION(o.name);
                    if (o.items != null)
                    {
                        for (int j = 0; j < o.items.Length; j++)
                        {
                            var oi = o.items[j];
                            h.FIELD(oi.name, box: 6).FIELD(oi.price, box: 0x23, sign: "¥").FIELD(oi.load, null, oi.unit, box: 0x23);
                        }
                    }
                    h.TAIL();
                });
            }
        }

        [Ui("新建"), Tool(ButtonConfirm), User(OPRSTAFF)]
        public void @new(ActionContext ac)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                var shop = ((SampService) Service).Shops[shopid];
                var o = new Order
                {
                    rev = 1,
                    status = 0,
                    shopid = shopid,
                    shopname = shop.name,
                    typ = POS,
                    min = shop.min,
                    notch = shop.notch,
                    off = shop.off,
                    created = DateTime.Now
                };
                const short proj = -1 ^ KEY ^ Order.LATER;
                dc.Execute(dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj), p => o.Write(p, proj), false);
            }
            ac.GiveRedirect();
        }

        [Ui("删除"), Tool(ButtonPickConfirm), User(OPRSTAFF)]
        public async Task del(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            int[] key = (await ac.ReadAsync<Form>())[nameof(key)];
            if (key != null)
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(dc.Sql("DELETE FROM orders WHERE shopid = @1 AND id")._IN_(key), p => p.Set(shopid), false);
                }
            }
            ac.GiveRedirect();
        }
    }

    [Ui("新单")]
    public class OprNewlyWork : OrderWork<OprNewlyVarWork>
    {
        public OprNewlyWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("全部"), Tool(Anchor)]
        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            ac.GivePage(200, main =>
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query("SELECT * FROM orders WHERE status = " + PAID + " AND shopid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20));
                    main.TOOLBAR();
                    main.BOARDVIEW(dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("#").T(o.id).SEP().T(o.paid)._CAPTION();
                        h.FIELD_("收货").T(o.name)._T(o.addr)._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.FIELD(oi.name, box: 4).FIELD(oi.price, box: 4).FIELD(oi.qty, null, oi.unit, box: 4);
                        }
                        h.FIELD_(box: 8)._FIELD().FIELD(o.total, "总计", box: 4);
                        h.TAIL(o.Err(), false);
                    });
                }
            }, false, 3);
        }

        [Ui("区域"), Tool(AnchorPrompt)]
        public void area(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            bool inner = ac.Query[nameof(inner)];
            string filter = (string) ac.Query[nameof(filter)] ?? string.Empty;
            if (inner)
            {
                ac.GivePane(200, m =>
                {
                    var shop = ((SampService) Service).Shops[shopid];
                    m.FORM_();
                    m.RADIOSET(nameof(filter), filter, shop.areas);
                    m._FORM();
                });
                return;
            }
            ac.GivePage(200, main =>
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Query("SELECT * FROM orders WHERE status = " + PAID + " AND shopid = @1 AND addr LIKE @2 ORDER BY id DESC LIMIT 20 OFFSET @3", p => p.Set(shopid).Set(filter + "%").Set(page * 20));
                    main.TOOLBAR(title: filter);
                    main.BOARDVIEW(dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CAPTION_().T("#").T(o.id).SEP().T(o.paid)._CAPTION();
                        h.FIELD_("收货").T(o.name)._T(o.addr)._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.FIELD(oi.name, box: 4).FIELD(oi.price, box: 4).FIELD(oi.qty, null, oi.unit, box: 4);
                        }
                        h.FIELD_(box: 8)._FIELD().FIELD(o.total, "总计", box: 4);
                        h.TAIL(o.Err(), false);
                    });
                }
            }, false, 3);
        }

        static readonly Map<string, string> MSGS = new Map<string, string>
        {
            ["订单处理"] = "我们已经接到您的订单（金额{0}元）",
            ["派送通知"] = "销售人员正在派送您所购的商品",
            ["sdf"] = "",
        };

        [Ui("通知"), Tool(ButtonPickShow)]
        public void send(ActionContext ac)
        {
            long[] key = ac.Query[nameof(key)];
            string msg = null;
            if (ac.GET)
            {
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.RADIOSET(nameof(msg), msg, MSGS, "消息通知买家", box: 0x4c);
                    m._FORM();
                });
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute(dc.Sql("SELECT wx FROM orders WHERE id")._IN_(key), prepare: false);
                }
                ac.GivePane(200);
            }
        }
    }

    [Ui("旧单"), User(OPR)]
    public class OprPastlyWork : OrderWork<OprPastlyVarWork>
    {
        public OprPastlyWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string shopid = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status > " + PAID + " AND shopid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(shopid).Set(page * 20));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("#").T(o.id).SEP().T(o.paid)._CAPTION(Statuses[o.status], o.status == FINISHED);
                    h.FIELD_("收货").T(o.name)._T(o.addr)._T(o.tel)._FIELD();
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var oi = o.items[i];
                        h.FIELD(oi.name, box: 6).FIELD(oi.price, sign: "¥", box: 0x23).FIELD(oi.qty, sign: oi.unit, box: 3);
                    }
                    h.BOX_(9)._BOX().FIELD(o.total, "总价", sign: "¥", box: 3);
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
                dc.Execute(dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key));
            }
            ac.GiveRedirect();
        }
    }

    [Ui("反馈")]
    [User(adm: true)]
    public class AdmKickWork : OrderWork<AdmKickVarWork>
    {
        public AdmKickWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM orders WHERE status > 0 AND kick IS NOT NULL ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(page * 20));
                ac.GiveBoardPage(200, dc.ToArray<Order>(), (h, o) =>
                {
                    h.CAPTION_().T("#")._T(o.id).SEP().T(o.paid)._CAPTION();
                    if (o.name != null)
                    {
                        //                        h.FIELD(o.name, "姓名", box: 6).FIELD(o.city, "城市", box: 6);
                    }
                    h.BOX_().T(o.tel)._T(o.addr)._BOX();
                    for (int i = 0; i < o.items.Length; i++)
                    {
                        var it = o.items[i];
                        h.FIELD(it.name, box: 4).FIELD(it.price, box: 4).FIELD(it.qty, sign: it.unit, box: 4);
                    }
                    h.FIELD(o.total, "总价");
                }, false, 3);
            }
        }
    }
}