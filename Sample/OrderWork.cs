using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.Order;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    public abstract class OrderWork<V> : Work where V : OrderVarWork
    {
        protected OrderWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, long>((obj) => ((Order) obj).id);
        }
    }

    [Ui("购物车, 订单")]
    public class MyOrderWork : OrderWork<MyOrderVarWork>
    {
        public MyOrderWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac, int page)
        {
            string wx = ac[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("(SELECT * FROM orders WHERE wx = @1 AND status <= 1 ORDER BY id DESC) UNION ALL (SELECT * FROM orders WHERE wx = @1 AND status > 1 ORDER BY id DESC)", p => p.Set(wx));
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();

                    bool bgn = false;
                    m.GRID_();
                    foreach (var o in arr)
                    {
                        if (!bgn && o.status > 1)
                        {
                            bgn = true;
                            m.H4("历史订单");
                        }
                        m.CARD_(o);
                        m.CARD_HEADER_().T(o.orgname)._IF(o.paid)._CARD_HEADER(Statuses[o.status], o.status <= PAID);

                        m.CARD_BODY_();
                        m.P_("收货").T(o.city).T(o.addr)._T(o.name).T(o.tel)._P().P_().VARTOOL("addr", when: o.status == 0)._P();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            if (o.status <= 1)
                            {
                                m.ICON("/org/" + o.orgid + "/" + oi.name + "/icon");
                                m.P(oi.name).P(oi.price, fix: "¥");
                                m.P(oi.qty, fix: oi.unit).VARTOOL("item", i, when: o.status == 0);
                                m.P(oi.load, fix: oi.unit, when: o.typ == POS);
                            }
                            else
                            {
                                m.FIELD_().T(oi.name)._T("¥").T(oi.price)._T(oi.qty).T(oi.unit)._FIELD();
                            }
                        }
                        m.FIELD(o.min + "元起订，每满" + o.notch + "元立减" + o.off + "元", width: 8);
                        m.FIELD(o.total, "总计", fix: "¥", tag: o.status == 0 ? "em" : null, width: 4);
                        m._CARD_BODY();

                        m.CARD_FOOTER(o.Err(), flag: o.status == 0 ? (byte) 1 : (byte) 0);

                        m._CARD();
                    }
                    m._GRID(arr?.Length ?? 0);
                }, false, 2);
            }
        }

        [Ui("清空购物车"), Tool(ButtonConfirm)]
        public void clear(WebContext ac)
        {
            string wx = ac[-1];
            using (var dc = NewDbContext())
            {
                dc.Execute("DELETE FROM orders WHERE wx = @1 AND status = 0", p => p.Set(wx));
            }
            ac.GiveRedirect();
        }
    }

    [Ui("销售摊点管理")]
    public class OprCartWork : OrderWork<OprCartVarWork>
    {
        public OprCartWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac)
        {
            string orgid = ac[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status = 0 AND orgid = @1 AND typ = 1", p => p.Set(orgid));
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.GRIDVIEW(arr, (h, o) =>
                    {
                        h.CARD_HEADER_().T("No.").T(o.id).SEP().T(o.addr)._CARD_HEADER(o.name);
                        if (o.items != null)
                        {
                            for (int j = 0; j < o.items.Length; j++)
                            {
                                var oi = o.items[j];
                                h.FIELD(oi.name, width: 6).FIELD(oi.price, fix: "¥", width: 0x23).FIELD(oi.load, null, oi.unit, width: 0x23);
                            }
                        }
                        h.CARD_FOOTER();
                    });
                });
            }
        }

        [Ui("新建"), Tool(ButtonConfirm), User(OPRSTAFF)]
        public void @new(WebContext ac)
        {
            string orgid = ac[-1];
            using (var dc = NewDbContext())
            {
                var org = Obtain<Map<string, Org>>()[orgid];
                var o = new Order
                {
                    rev = 1,
                    status = 0,
                    orgid = orgid,
                    orgname = org.name,
                    typ = POS,
                    min = org.min,
                    notch = org.notch,
                    off = org.off,
                    created = DateTime.Now
                };
                const byte proj = 0xff ^ KEY ^ Order.LATER;
                dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                dc.Execute(p => o.Write(p, proj), false);
            }
            ac.GiveRedirect();
        }

        [Ui("删除"), Tool(ButtonPickConfirm), User(OPRSTAFF)]
        public async Task del(WebContext ac, int page)
        {
            string orgid = ac[-1];
            int[] key = (await ac.ReadAsync<Form>())[nameof(key)];
            if (key != null)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("DELETE FROM orders WHERE orgid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(orgid), false);
                }
            }
            ac.GiveRedirect();
        }
    }

    [Ui("新订单")]
    public class OprNewoWork : OrderWork<OprNewoVarWork>
    {
        public OprNewoWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("全部"), Tool(Anchor)]
        public void @default(WebContext ac, int page)
        {
            string orgid = ac[-1];
            ac.GivePage(200, main =>
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT * FROM orders WHERE status = " + PAID + " AND orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                    main.TOOLBAR();
                    main.GRIDVIEW(dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CARD_HEADER_().T("No.").T(o.id).SEP().T(o.paid)._CARD_HEADER();
                        h.FIELD_("收货").T(o.name)._T(o.addr)._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.FIELD(oi.name, width: 4).FIELD(oi.price, width: 4).FIELD(oi.qty, null, oi.unit, width: 4);
                        }
                        h.FIELD_(width: 8)._FIELD().FIELD(o.total, "总计", width: 4);
                        h.CARD_FOOTER(o.Err(), 'w');
                    });
                }
            }, false, 3);
        }

        [Ui("按区域"), Tool(AnchorPrompt)]
        public void area(WebContext ac, int page)
        {
            string orgid = ac[-1];
            bool inner = ac.Query[nameof(inner)];
            string filter = (string) ac.Query[nameof(filter)] ?? string.Empty;
            if (inner)
            {
                ac.GivePane(200, m =>
                {
                    var org = Obtain<Map<string, Org>>()[orgid];
                    m.FORM_();
                    m.RADIOSET(nameof(filter), filter, org.areas);
                    m._FORM();
                });
                return;
            }
            ac.GivePage(200, main =>
            {
                using (var dc = NewDbContext())
                {
                    dc.Query("SELECT * FROM orders WHERE status = " + PAID + " AND orgid = @1 AND addr LIKE @2 ORDER BY id DESC LIMIT 20 OFFSET @3", p => p.Set(orgid).Set(filter + "%").Set(page * 20));
                    main.TOOLBAR(title: filter);
                    main.GRIDVIEW(dc.ToArray<Order>(), (h, o) =>
                    {
                        h.CARD_HEADER_().T("No.").T(o.id).SEP().T(o.paid)._CARD_HEADER();
                        h.FIELD_("收货").T(o.name)._T(o.addr)._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.FIELD(oi.name, width: 4).FIELD(oi.price, width: 4).FIELD(oi.qty, null, oi.unit, width: 4);
                        }
                        h.FIELD_(width: 8)._FIELD().FIELD(o.total, "总计", width: 4);
                        h.CARD_FOOTER(o.Err(), 'w');
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
        public void send(WebContext ac)
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
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT wx FROM orders WHERE id")._IN_(key);
                    dc.Execute(prepare: false);
                }
                ac.GivePane(200);
            }
        }
    }

    [Ui("旧订单"), User(OPR)]
    public class OprOldoWork : OrderWork<OprOldoVarWork>
    {
        public OprOldoWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac, int page)
        {
            string orgid = ac[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Order>("SELECT * FROM orders WHERE status > " + PAID + " AND orgid = @1 ORDER BY id DESC LIMIT 20 OFFSET @2", p => p.Set(orgid).Set(page * 20));
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.GRIDVIEW(arr, (h, o) =>
                    {
                        h.CARD_HEADER_().T("No.").T(o.id).SEP().T(o.paid)._CARD_HEADER(Statuses[o.status], o.status == FINISHED);
                        h.FIELD_("收货").T(o.name)._T(o.addr)._T(o.tel)._FIELD();
                        for (int i = 0; i < o.items.Length; i++)
                        {
                            var oi = o.items[i];
                            h.FIELD(oi.name, width: 6).FIELD(oi.price, fix: "¥", width: 0x23).FIELD(oi.qty, fix: oi.unit, width: 3);
                        }
                        h.FIELD(o.total, "总价", fix: "¥", width: 3);
                        h.CARD_FOOTER();
                    });
                }, false, 2);
            }
        }

        [Ui("查询"), Tool(AnchorShow)]
        public void send(WebContext ac)
        {
            long[] key = ac.Query[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE orders SET status = @1 WHERE id")._IN_(key);
                dc.Execute();
            }
            ac.GiveRedirect();
        }

        [Ui("回退", "【警告】把选中的订单回退成新单？"), Tool(ButtonPickConfirm)]
        public async Task back(WebContext ac)
        {
            string orgid = ac[-2];
            var f = await ac.ReadAsync<Form>();
            string[] key = f[nameof(key)];
            if (key != null)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE orders SET status = ").T(PAID).T(" WHERE status > ").T(PAID).T(" AND orgid = @1 AND id")._IN_(key);
                    dc.Execute(p => p.Set(orgid), prepare: false);
                }
            }
            ac.GiveRedirect();
        }
    }
}