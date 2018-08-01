using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class ItemVarWork : Work
    {
        const int PICAGE = 60 * 60;

        protected ItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext wc)
        {
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE name = @1", p => p.Set(name)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PICAGE);
                }
                else wc.Give(404, @public: true, maxage: PICAGE); // not found
            }
        }

        public void img(WebContext wc, int ordinal)
        {
            string orgid = wc[-1];
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PICAGE);
                }
                else wc.Give(404, @public: true, maxage: PICAGE); // not found
            }
        }
    }

    public class SampVarWork : ItemVarWork
    {
        public SampVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string name = wc[this];
            var o = Obtain<Map<string, Item>>()[name];
            wc.GivePane(200, h =>
            {
                // movie


                // remark
                h.ALERT(o.remark);


                // schedule
            });
        }

        [UserAccess(true)]
        [Ui("购买"), Tool(AOpen), ItemState('A')]
        public async Task buy(WebContext wc)
        {
            User prin = (User) wc.Principal;
            string itemname = wc[this];
            var item = Obtain<Map<string, Item>>()[itemname];
            short num;
            if (wc.GET)
            {
                int posid = wc.Query[nameof(posid)];
                wc.GivePane(200, h =>
                {
                    using (var dc = NewDbContext())
                    {
                        h.FORM_();
                        if (posid > 0) // if in POS mode
                        {
                            h.HIDDEN(nameof(posid), posid);
                        }
                        // quantity
                        h.FIELDSET_("加入货品");
                        h.LI_("货　品").ICO_("uk-width-1-6").T("icon")._ICO().SP().T(item.name)._LI();
                        h.LI_("数　量").NUMBER(nameof(num), posid > 0 ? 1 : item.min, min: posid > 0 ? 1 : item.min, max: item.demand, step: posid > 0 ? 1 : item.step).T(item.unit)._LI();
                        h._FIELDSET();

                        h.BOTTOMBAR_().TOOL(nameof(prepay))._BOTTOMBAR();

                        h._FORM();
                    }
                });
            }
            else // POST
            {
                using (var dc = NewDbContext())
                {
                    const byte proj = 0xff ^ Order.KEY ^ Order.LATER;
                    var f = await wc.ReadAsync<Form>();
                    string posid = f[nameof(posid)];
                    var o = new Order
                    {
                        uid = prin.id,
                        uname = prin.name,
                        uwx = prin.wx,
                        created = DateTime.Now
                    };
                    o.Read(f, proj);
                    num = f[nameof(num)];
//                        o.AddItem(itemname, item.unit, item.price, num);
                    dc.Sql("INSERT INTO orders ")._(o, proj)._VALUES_(o, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                wc.GivePane(200, m =>
                {
                    m.MSG_(true, "成功加入购物车", "商品已经成功加入购物车");
                    m.BOTTOMBAR_().A_GOTO("去付款", "cart", href: "/my//ord/")._BOTTOMBAR();
                });
            }
        }

        [Ui("付款"), Tool(ButtonScript, Style.Primary), OrderState('P')]
        public async Task prepay(WebContext wc)
        {
            var prin = (User) wc.Principal;
            int orderid = wc[this];
            Order o;
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM orders WHERE id = @1 AND custid = @2");
                o = dc.Query1<Order>(p => p.Set(orderid).Set(prin.id));
            }
            var (prepay_id, _) = await ((SampService) Service).WeiXin.PostUnifiedOrderAsync(
                orderid + "-",
                o.cash,
                prin.wx,
                wc.RemoteAddr.ToString(),
                SampUtility.NETADDR + "/" + nameof(SampService.onpay),
                "粗粮达人-健康产品"
            );
            if (prepay_id != null)
            {
                wc.Give(200, ((SampService) Service).WeiXin.BuildPrepayContent(prepay_id));
            }
            else
            {
                wc.Give(500);
            }
        }
    }

    public class CtrItemVarWork : ItemVarWork
    {
        public CtrItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [UserAccess(CTR_MGR)]
        [Ui("修改"), Tool(ButtonShow, size: 2)]
        public async Task upd(WebContext wc)
        {
            string orgid = wc[-2];
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE orgid = @1 AND name = @2", p => p.Set(orgid).Set(name));
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("填写货品信息");
                        h.STATIC(o.name, "名称");
                        h.TEXTAREA(nameof(o.descr), o.descr, "描述", min: 20, max: 50, required: true);
                        h.TEXT(nameof(o.unit), o.unit, "单位", required: true);
                        h.NUMBER(nameof(o.price), o.price, "单价", required: true);
                        h.NUMBER(nameof(o.min), o.min, "起订", min: (short) 1);
                        h.NUMBER(nameof(o.step), o.step, "增减", min: (short) 1);
                        h.SELECT(nameof(o.status), o.status, Item.Statuses, "状态");
                        h.NUMBER(nameof(o.demand), o.demand, "可供");
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0xff ^ Item.PK;
                var o = await wc.ReadObjectAsync<Item>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE orgid = @1 AND name = @2");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(orgid).Set(name);
                    });
                }
                wc.GivePane(200); // close
            }
        }

        [UserAccess(CTR_MGR)]
        [Ui("照片"), Tool(ButtonCrop, size: 4)]
        public new async Task icon(WebContext wc)
        {
            if (wc.GET)
            {
                base.icon(wc);
            }
            else // POST
            {
                string orgid = wc[-2];
                string name = wc[this];
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE orgid = @2 AND name = @3", p => p.Set(img).Set(orgid).Set(name)) > 0)
                    {
                        wc.Give(200); // ok
                    }
                    else wc.Give(500); // internal server error
                }
            }
        }

        [Ui("计划"), Tool(ButtonShow, size: 4)]
        public async Task plan(WebContext wc)
        {
        }
    }
}