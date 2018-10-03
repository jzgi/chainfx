using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class ItemVarWork : Work
    {
        const int PicAge = 60 * 60;

        protected ItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void icon(WebContext wc)
        {
            short id = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT icon FROM items WHERE id = @1", p => p.Set(id)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PicAge);
                }
                else wc.Give(404, @public: true, maxage: PicAge); // not found
            }
        }

        public void img(WebContext wc, int ordinal)
        {
            short id = wc[this];
            using (var dc = NewDbContext())
            {
                if (dc.Query1("SELECT img" + ordinal + " FROM items WHERE id = @2", p => p.Set(id)))
                {
                    dc.Let(out byte[] bytes);
                    if (bytes == null) wc.Give(204); // no content 
                    else wc.Give(200, new StaticContent(bytes), true, PicAge);
                }
                else wc.Give(404, @public: true, maxage: PicAge); // not found
            }
        }
    }


    public class SampItemVarWork : ItemVarWork
    {
        public SampItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[-1];
            string name = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE hubid = @1 AND name = @2");
                var o = dc.Query1<Item>(p => p.Set(hubid).Set(name));
                wc.GivePage(200, h => { });
            }
        }

        [Ui("购买", "订购商品"), Tool(AnchorOpen, size: 1, access: false), ItemState('A')]
        public async Task buy(WebContext wc)
        {
            User prin = (User) wc.Principal;
            string name = wc[this];
            var item = Obtain<Map<string, Item>>()[name];
            short num;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    bool ingrp = prin.teamat != null;
                    using (var dc = NewDbContext())
                    {
                        h.FORM_();
                        // quantity
                        h.FIELDUL_("加入货品");
                        h.LI_().ICO_("uk-width-1-6").T("icon")._ICO().SP().T(item.name)._LI();
                        h.LI_().NUMBER(null, nameof(num), item.min, max: item.queue, min: item.min, step: item.step).T(item.unit)._LI();
                        h._FIELDUL();

                        h.BOTTOMBAR_().TOOL(nameof(prepay))._BOTTOMBAR();

                        h._FORM();
                    }
                });
            }
            else // POST
            {
                using (var dc = NewDbContext())
                {
                    const byte proj = 0xff ^ Order.ID ^ Order.LATER;
                    var f = await wc.ReadAsync<Form>();
                    string posid = f[nameof(posid)];
                    var o = new Order
                    {
                        uid = prin.id,
                        uname = prin.name,
                        uwx = prin.wx,
                        paid = DateTime.Now
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

        [Ui("付款"), Tool(ButtonScript, "uk-button-primary"), OrderState('P')]
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
            //            var (prepay_id, _) = await ((SampService) Service).Hub.PostUnifiedOrderAsync(
            //                orderid + "-",
            //                o.cash,
            //                prin.wx,
            //                wc.RemoteAddr.ToString(),
            //                SampUtility.NETADDR + "/" + nameof(SampService.onpay),
            //                "粗粮达人-健康产品"
            //            );
            //            if (prepay_id != null)
            //            {
            //                wc.Give(200, ((SampService) Service).Hub.BuildPrepayContent(prepay_id));
            //            }
            //            else
            //            {
            //                wc.Give(500);
            //            }
        }
    }

    public class HubItemVarWork : ItemVarWork
    {
        public HubItemVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [UserAccess(7)]
        [Ui("资料", "填写货品资料"), Tool(ButtonShow, size: 2)]
        public async Task upd(WebContext wc)
        {
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE name = @1 ORDER BY status DESC, name", p => p.Set(name));
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDUL_(o.name);
                        h.LI_().TEXTAREA("简　介", nameof(o.descr), o.descr, max: 100, min: 20, required: true)._LI();
                        h.LI_().TEXTAREA("说　明", nameof(o.remark), o.descr, max: 500, min: 100, required: true)._LI();
                        h.LI_().URL("视　频", nameof(o.mov), o.mov)._LI();
                        h.LI_().TEXT("单　位", nameof(o.unit), o.unit, required: true)._LI();
                        h.LI_().NUMBER("单　价", nameof(o.price), o.price, required: true).LABEL("供应价").NUMBER(null, nameof(o.shopp), o.shopp, required: true)._LI();
                        h.LI_().NUMBER("派送费", nameof(o.fee), o.fee, required: true).LABEL("团组费").NUMBER(null, nameof(o.fee), o.fee, required: true)._LI();
                        h.LI_().NUMBER("起　订", nameof(o.min), o.min, min: (short) 1).LABEL("增　减").NUMBER(null, nameof(o.step), o.step, min: (short) 1)._LI();
                        h.LI_().LABEL("冷　藏").CHECKBOX(nameof(o.refrig), o.refrig)._LI();
                        h._FIELDUL();
                        h._FORM();
                    });
                }
            }
            else // POST
            {
                const byte proj = 0;
                var o = await wc.ReadObjectAsync<Item>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE items")._SET_(Item.Empty, proj).T(" WHERE name = @1");
                    dc.Execute(p =>
                    {
                        o.Write(p, proj);
                        p.Set(name);
                    });
                }
                wc.GivePane(200); // close
            }
        }

        [UserAccess(7)]
        [Ui("图片"), Tool(ButtonCrop, size: 1)]
        public new async Task icon(WebContext wc)
        {
            if (wc.GET)
            {
                base.icon(wc);
            }
            else // POST
            {
                string name = wc[this];
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> img = f[nameof(img)];
                using (var dc = NewDbContext())
                {
                    if (dc.Execute("UPDATE items SET icon = @1 WHERE name = @2", p => p.Set(img).Set(name)) > 0)
                    {
                        wc.Give(200); // ok
                    }
                    else wc.Give(500); // internal server error
                }
            }
        }

        [Ui("调度"), Tool(ButtonShow, size: 4)]
        public async Task plan(WebContext wc)
        {
            string name = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<Item>("SELECT * FROM items WHERE name = @1", p => p.Set(name));
                    wc.GivePane(200, h =>
                    {
                        string tel = null;
                        h.FORM_();
                        h.FIELDUL_("填写供货信息");
                        h.TEL("手机", nameof(tel), tel);
                        h.TEXT("单位", nameof(o.unit), o.unit, required: true);
                        h.SELECT("状态", nameof(o.status), o.status, Item.Statuses);
                        h._FIELDUL();
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
                    dc.Execute(p => { o.Write(p, proj); });
                }
                wc.GivePane(200); // close
            }
        }
    }
}