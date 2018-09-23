using System;
using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    [UserAuth]
    public class SampVarWork : ItemVarWork
    {
        public SampVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<SampChatWork>("chat"); // chat

            Create<MyWork>("my"); // personal

            Create<TeamWork>("team"); // customer team

            Create<ShopWork>("shop"); // supplying shop

            Create<RegWork>("hub"); // central 
        }

        public async Task @catch(WebContext wc, int cmd)
        {
            if (cmd == 1) // handle form submission
            {
                var o = (User) wc.Principal;
                var f = await wc.ReadAsync<Form>();
                o.Read(f);
                string url = f[nameof(url)];
                using (var dc = NewDbContext())
                {
                    const byte proj = 0xff ^ User.ID ^ User.LATER;
                    dc.Sql("INSERT INTO users ")._(User.Empty, proj)._VALUES_(User.Empty, proj).T(" ON CONFLICT () UPDATE SET ").setlst(User.Empty, proj);
                    dc.Execute(p => o.Write(p));
                    wc.SetTokenCookie(o, 0xff ^ User.PRIVACY);
                }
                wc.GiveRedirect(url);
            }
            else if (wc.Except is AccessException ace)
            {
                if (ace.Result == false && wc.Principal == null)
                {
                    // weixin authorization challenge
                    if (wc.ByWeiXinClient) // weixin
                    {
//                        Reg.GiveRedirectWeiXinAuthorize(wc, NETADDR);
                    }
                    else // challenge BASIC scheme
                    {
                        wc.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        wc.Give(401); // unauthorized
                    }
                }
                else if (ace.Result == null && wc.Principal != null)
                {
                    var o = (User) wc.Principal;
                    string url = wc.Path;
                    wc.GivePage(200, h =>
                    {
                        h.FORM_();
                        h.FIELDUL_("完善用户资料");
                        h.LI_().TEXT("用户名称", nameof(o.name), o.name, max: 4, min: 2, required: true)._LI();
                        h.LI_().TEXT("手　　机", nameof(o.tel), o.tel, pattern: "[0-9]+", max: 11, min: 11, required: true)._LI();
                        h.HIDDEN(nameof(url), url);
                        var orgs = Obtain<Map<string, Org>>();
//                        h.LI_().SELECT("参　　团", nameof(o.teamat), o.teamat, orgs, tip: "（无）", filter: x => x.hubid == 1)._LI();
                        h.LI_().TEXT("收货地址", nameof(o.addr), o.addr, max: 21, min: 2, required: true)._LI();
                        h._FIELDUL();
                        h.BOTTOMBAR_().BUTTON("/catch", 1, "确定", css: "uk-button-primary")._BOTTOMBAR();
                        h._FORM();
                    }, title: "用户注册");
                }
                else // IsNotAllowed
                {
                    wc.GivePage(403, h => { h.ALERT("您要使用的功能需要管理员授权。"); }, title: "没有访问权限");
                }
            }
            else
            {
                wc.Give(500, wc.Except.Message);
            }
        }

        public void @default(WebContext wc)
        {
            string regid = wc[this];
            wc.GivePage(200, h =>
                {
                    h.TOPBAR(true);

                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE regid = @1 AND status > 0");
                        var arr = dc.Query<Item>(p => p.Set(regid));
                        h.LIST(arr, o =>
                        {
                            h.T("<a class=\"uk-width-1-3 uk-margin-auto-vertical\" href=\"").T(o.name).T("/\" onclick=\"return dialog(this, 8, false, 4, '商品详情');\">");
                            h.ICO_(css: "uk-padding-small").T(o.name).T("/icon")._ICO();
                            h.T("</a>");
                            h.COL_(css: "uk-width-2-3 uk-padding-small");
                            h.H3(o.name);
                            h.FI(null, o.descr);
                            h.ROW_();
                            h.P_("uk-width-2-3").T("￥<em>").T(o.price).T("</em>／").T(o.unit)._P();
                            h.FORM_(css: "uk-width-auto");
                            h.TOOL(nameof(SampVarWork.buy));
                            h._FORM();
                            h._ROW();
                            h._COL();
                        }, "uk-padding-remove");
                    }
                }, true, 60
            );
        }


        [Ui("购买", "订购商品"), Tool(Modal.AOpen, size: 1, auth: false), ItemState('A')]
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
                        h.LI_().NUMBER(null, nameof(num), item.min, max: item.demand, min: item.min, step: item.step).T(item.unit)._LI();
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
                    const byte proj = 0xff ^ Order.KEY ^ Order.LATER;
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

        [Ui("付款"), Tool(Modal.ButtonScript, "uk-button-primary"), OrderState('P')]
        public async Task prepay(WebContext wc)
        {
            var prin = (User) wc.Principal;
            int orderid = wc[this];
            Order o;
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(User.Empty).T(" FROM orders WHERE id = @1 AND custid = @2");
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

        /// <summary>
        /// WCPay notify, without authentic context.
        /// </summary>
        public async Task onpay(WebContext wc)
        {
            XElem xe = await wc.ReadAsync<XElem>();
//            if (!Reg.OnNotified(xe, out var trade_no, out var cash))
//            {
//                wc.Give(400);
//                return;
//            }
//            var orderid = trade_no.ToInt();
//            string grpid, uname, uaddr;
//            // update order status
//            using (var dc = NewDbContext())
//            {
//                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = 1 WHERE id = @2 AND status = 0 RETURNING grpid, uname, uaddr", (p) => p.Set(cash).Set(orderid)))
//                {
//                    return; // WCPay may send notification more than once
//                }
//                dc.Let(out grpid).Let(out uname).Let(out uaddr);
//            }
//            // send message to the related grouper, if any
//            if (grpid != null)
//            {
//                var oprwx = Obtain<Map<string, Org>>()[grpid]?.mgrwx;
//                if (oprwx != null)
//                {
//                    await Reg.PostSendAsync(oprwx, "新订单", ("¥" + cash + " " + uname + " " + uaddr), NETADDR + "/grp//ord/");
//                }
//                // return xml to WCPay server
//                XmlContent x = new XmlContent(true, 1024);
//                x.ELEM("xml", null, () =>
//                {
//                    x.ELEM("return_code", "SUCCESS");
//                    x.ELEM("return_msg", "OK");
//                });
//                wc.Give(200, x);
//            }
        }
    }
}