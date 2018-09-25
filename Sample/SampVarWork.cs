using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    [UserAccess]
    [Ui("全粮派")]
    public class SampVarWork : Work
    {
        public SampVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<SampItemVarWork, string>();

            Create<SampChatWork>("chat"); // chat

            Create<MyWork>("my"); // personal

            Create<TeamWork>("team"); // customer team

            Create<ShopWork>("shop"); // supplying shop

            Create<RegWork>("reg"); // central 
        }

        public async Task @catch(WebContext wc, int cmd)
        {
            string regid = wc[0];
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
            else if (wc.Except is AccessException)
            {
                if (wc.Principal == null)
                {
                    // weixin authorization challenge
                    if (wc.ByWeiXinClient) // weixin
                    {
                        var reg = Obtain<Map<string, Reg>>()[regid];
                        reg.GiveRedirectWeiXinAuthorize(wc, SampUtility.NETADDR);
                    }
                    else // challenge BASIC scheme
                    {
                        wc.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        wc.Give(401); // unauthorized
                    }
                }
                else if (((User) wc.Principal).IsIncomplete)
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
                        h.LI_().SELECT("参　　团", nameof(o.teamat), o.teamat, orgs, tip: "（无）", filter: x => x.regid == regid)._LI();
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
//                            h.TOOL(nameof(buy));
                            h._FORM();
                            h._ROW();
                            h._COL();
                        }, "uk-padding-remove");
                    }
                }, true, 60
            );
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