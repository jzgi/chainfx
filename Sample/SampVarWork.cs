using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    [UserAuthenticate]
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
                        var map = dc.Query<string, Hub>();
                        // init weixin pay for each hub
                        for (int i = 0; i < map.Count; i++)
                        {
                            map[i].InitWCPay();
                        }
                        return map;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wc"></param>
        /// <param name="cmd">0 - being dispatched due to expcetion, 1 - form submission, 2 - form submission with suggested teamid, 3 - suggested sign up form</param>
        /// <returns></returns>
        public async Task @catch(WebContext wc, int cmd)
        {
            string hubid = wc[this];
            if (cmd == 0)
            {
                var o = (User) wc.Principal;
                if (wc.Except is AuthorizeException)
                {
                    if (o == null)
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
                    else if (o.IsTemporary)
                    {
                        GiveSignForm(o, wc.Path);
                    }
                    else
                    {
                        wc.GivePage(403, h => { h.ALERT("您要使用的功能需要管理员授权。"); }, title: "没有访问权限");
                    }
                }
                else
                {
                    wc.Give(500, wc.Except.Message);
                }
            }
            else if (cmd == 1 || cmd == 2) // handle form submission
            {
                var o = (User) wc.Principal ?? new User();
                o.hubid = hubid;
                var f = await wc.ReadAsync<Form>();
                o.Read(f);
                string url = f[nameof(url)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO users ")._(User.Empty, 0)._VALUES_(User.Empty, 0).T(" ON CONFLICT (tel) DO UPDATE SET ").setlst(User.Empty, 0).T(" WHERE users.wx IS NULL AND users.tel = @tel AND users.name = @name");
                    if (dc.Execute(p => o.Write(p, 0)) > 0)
                    {
                        wc.SetTokenCookie(o, 0xff ^ User.PRIVACY);
                    }
                }
                if (cmd == 2) // if was saved with suggested teamid
                {
                    wc.SetTokenCookie(o, 0xff ^ User.PRIVACY);
                    var hub = Obtain<Map<string, Hub>>()[hubid];
                    wc.GiveRedirect(hub.watchurl); // redirect to the weixin account watch page
                }
                else // was opened manually
                {
                    wc.GiveRedirect(url);
                }
            }
            else
            {
                var o = (User) wc.Principal;
                short teamid = wc.Query[nameof(teamid)];
                GiveSignForm(o ?? new User() {teamid = teamid}, wc.Path);
            }

            void GiveSignForm(User o, string url)
            {
                wc.GivePage(200, h =>
                {
                    h.FORM_();

                    h.HIDDEN(nameof(o.wx), o.wx);
                    h.HIDDEN(nameof(url), url);

                    h.FIELDUL_("填写用户信息");
                    h.LI_().TEXT("手　　机", nameof(o.tel), o.tel, pattern: "[0-9]+", max: 11, min: 11, required: true)._LI();
                    h.LI_().TEXT("您的姓名", nameof(o.name), o.name, max: 4, min: 2, required: true)._LI();
                    var orgs = Obtain<Map<short, Team>>();
                    h.LI_().SELECT("参　　团", nameof(o.teamid), o.teamid, orgs, filter: x => x.hubid == hubid)._LI();
                    h.LI_().TEXT("收货地址", nameof(o.addr), o.addr, max: 30, required: true)._LI();
                    h._FIELDUL();
                    h.BOTTOMBAR_().BUTTON("确定", "/" + hubid + "/catch-2", css: "uk-button-primary")._BOTTOMBAR();
                    h._FORM();
                }, title: "填写用户信息");
            }
        }

        [UserAuthorize]
        public void @default(WebContext wc)
        {
            string hubid = wc[this];
            var hub = Obtain<Map<string, Hub>>()[hubid];
            int uid = wc.Query[nameof(uid)];
            wc.GivePage(200, h =>
                {
                    h.TOPBAR(true, uid);
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE hubid = @1 AND status > 0");
                        var arr = dc.Query<Item>(p => p.Set(hubid));

                        // load availability
                        var map = new Map<short, short>();
                        dc.Query("SELECT itemid, sum(qty) FROM orders WHERE hubid = @1 AND status BETWEEN 0 AND 5 GROUP BY itemid", p => p.Set(hubid));
                        while (dc.Next())
                        {
                            dc.Let(out short itemid).Let(out short qty);
                            map.Add(itemid, qty);
                        }
                        foreach (var a in arr) a.ongoing = map[a.id];

                        h.LIST(arr, o =>
                        {
                            h.T("<a class=\"uk-grid uk-width-1-1 uk-link-reset\" href=\"").T(o.id).T("/");
                            if (uid > 0)
                            {
                                h.T("?uid=").T(uid);
                            }
                            h.T("\" onclick=\"return dialog(this, 8, false, 2, '商品详情');\">");
                            h.PIC_(css: "uk-width-1-3 uk-padding-small").T(o.id).T("/icon")._PIC();
                            h.COL_(css: "uk-width-2-3 uk-padding-small");
                            h.H3(o.name);
                            h.FI(null, o.descr);
                            h.ROW_();
                            h.P_("").T("￥<em>").T(o.price).T("</em>／").T(o.unit)._P();
                            h.PROGRESS(o.ongoing, o.cap7);
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
            string hubid = wc[0];
            var hub = Obtain<Map<string, Hub>>()[hubid];
            XElem xe = await wc.ReadAsync<XElem>();
            if (!hub.OnNotified(xe, out var trade_no, out var cash))
            {
                wc.Give(400);
                return;
            }
            var orderid = trade_no.ToInt();
            // update order status
            using (var dc = NewDbContext())
            {
                // WCPay may send notification more than once
                dc.Sql("UPDATE orders SET cash = @1, paid = localtimestamp, status = 1 WHERE id = @2 AND status = 0");
                dc.Execute(p => p.Set(cash).Set(orderid));
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