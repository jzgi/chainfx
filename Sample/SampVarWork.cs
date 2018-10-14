using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    [UserAccess]
    public class SampVarWork : Work
    {
        public SampVarWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<SampItemVarWork>();

            Make<SampChatWork>("chat"); // chat
            Make<MyWork>("my"); // personal
            Make<TeamlyWork>("team"); // customer team
            Make<ShoplyWork>("shop"); // workshop
            Make<HublyWork>("hub"); // central 

            // register cached active central hubs
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Hub.Empty).T(" FROM hubs WHERE status > 0 ORDER BY name");
                        return dc.Query<string, Hub>();
                    }
                }, 3600
            );
            // register cached active workshops
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Shop.Empty).T(" FROM shops WHERE status > 0 ORDER BY hubid, name");
                        return dc.Query<short, Shop>();
                    }
                }, 300
            );
            // register cached active customer teams
            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Team.Empty).T(" FROM teams WHERE status > 0 ORDER BY hubid, name");
                        return dc.Query<short, Team>();
                    }
                }, 300
            );
        }

        public async Task @catch(WebContext wc, int cmd)
        {
            string hubid = wc[this];
            if (cmd == 1) // handle form submission
            {
                var o = (User) wc.Principal;
                var f = await wc.ReadAsync<Form>();
                o.Read(f);
                string url = f[nameof(url)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO users ")._(User.Empty, 0)._VALUES_(User.Empty, 0).T(" ON CONFLICT () UPDATE SET ").setlst(User.Empty, 0);
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
                        var hub = Obtain<Map<string, Hub>>()[hubid];
                        hub.GiveRedirectWeiXinAuthorize(wc, SampUtility.NetAddr);
                    }
                    else // challenge BASIC scheme
                    {
                        wc.SetHeader("WWW-Authenticate", "Basic realm=\"APP\"");
                        wc.Give(401); // unauthorized
                    }
                }
                else if (!((User) wc.Principal).IsTeamed)
                {
                    var o = (User) wc.Principal;
                    string url = wc.Path;
                    wc.GivePage(200, h =>
                    {
                        h.FORM_();
                        h.FIELDUL_("填写用户信息");
                        h.LI_().TEXT("用户名称", nameof(o.name), o.name, max: 4, min: 2, required: true)._LI();
                        h.LI_().TEXT("手　　机", nameof(o.tel), o.tel, pattern: "[0-9]+", max: 11, min: 11, required: true)._LI();
                        h.HIDDEN(nameof(url), url);
                        var orgs = Obtain<Map<short, Team>>();
                        h.LI_().SELECT("参　　团", nameof(o.teamid), o.teamid, orgs, filter: x => x.hubid == hubid)._LI();
                        h.LI_().TEXT("收货地址", nameof(o.addr), o.addr, max: 30, required: true)._LI();
                        h._FIELDUL();
                        h.BOTTOM_().BUTTON("确定", "/catch", css: "uk-button-primary")._BOTTOM();
                        h._FORM();
                    }, title: "填写用户信息");
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
            string hubid = wc[this];
            var hub = Obtain<Map<string, Hub>>()[hubid];
            wc.GivePage(200, h =>
                {
                    h.TOPBAR(true);

                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE hubid = @1 AND status > 0");
                        var arr = dc.Query<Item>(p => p.Set(hubid));
                        h.LIST(arr, o =>
                        {
                            h.T("<a class=\"uk-grid uk-width-1-1 uk-link-reset\" href=\"").T(o.id).T("/\" onclick=\"return dialog(this, 8, false, 2, '商品详情');\">");
                            h.ICO_(css: "uk-width-1-3 uk-padding-small").T(o.id).T("/icon")._ICO();
                            h.COL_(css: "uk-width-2-3 uk-padding-small");
                            h.H3(o.name);
                            h.FI(null, o.descr);
                            h.ROW_();
                            h.P_("uk-width-2-3").T("￥<em>").T(o.price).T("</em>／").T(o.unit)._P();
                            h._ROW();
                            h._COL();
                            h.T("</a>");
                        }, "uk-padding-remove");
                    }
                }, true, 12, title: hub.name
            );
        }


        /// <summary>
        /// A WCPay payment notification
        /// </summary>
        public async Task onpay(WebContext wc)
        {
            string hubid = wc[this];
            var hub = Obtain<Map<string, Hub>>()[hubid];
            XElem xe = await wc.ReadAsync<XElem>();
            if (!hub.OnNotified(xe, out var trade_no, out var cash))
            {
                wc.Give(400);
                return;
            }
            var orderid = trade_no.ToInt();
            string teamid, uname, uaddr;
            // update order status
            using (var dc = NewDbContext())
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = 1 WHERE id = @2 AND status = 0 RETURNING grpid, uname, uaddr", (p) => p.Set(cash).Set(orderid)))
                {
                    return; // WCPay may send notification more than once
                }
                dc.Let(out teamid).Let(out uname).Let(out uaddr);
            }
            // send message to the related grouper, if any
            if (teamid != null)
            {
                var oprwx = Obtain<Map<string, Team>>()[teamid]?.mgrwx;
                if (oprwx != null)
                {
                    await hub.PostSendAsync(oprwx, "新订单", ("¥" + cash + " " + uname + " " + uaddr), SampUtility.NetAddr + "/grp//ord/");
                }
                // return xml to WCPay server
                XmlContent x = new XmlContent(true, 1024);
                x.ELEM("xml", null, () =>
                {
                    x.ELEM("return_code", "SUCCESS");
                    x.ELEM("return_msg", "OK");
                });
                wc.Give(200, x);
            }
        }
    }
}