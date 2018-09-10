using System;
using System.Text;
using System.Threading.Tasks;
using Greatbone;
using static Samp.SampUtility;

namespace Samp
{
    /// <summary>
    /// The sample service that hosts all functionalities of the app.
    /// </summary>
    [Ui("全粮派")]
    public class SampService : Service<User>, IAuthenticateAsync
    {
        readonly WeiXin weixin;

        public SampService(ServiceConfig cfg) : base(cfg)
        {
            CreateVar<SampVarWork, string>(obj => ((Item) obj).name);

            Create<SampChatWork>("chat"); // chat

            Create<MyWork>("my"); // personal

            Create<TeamWork>("team"); // customer team

            Create<ShopWork>("shop"); // supplying shop

            Create<HubWork>("hub"); // central 


            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs WHERE status > 0 ORDER BY typ, name");
                        return dc.Query<string, Org>(proj: 0xff);
                    }
                }, 1800
            );

            Register(delegate
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Item.Empty).T(" FROM items WHERE status > 0 ORDER BY status DESC, name");
                        return dc.Query<string, Item>(proj: 0xff);
                    }
                }, 120
            );

            weixin = DataUtility.FileToObject<WeiXin>(cfg.GetFilePath("$weixin.json"));
        }

        public WeiXin WeiXin => weixin;

        public async Task<bool> AuthenticateAsync(WebContext wc)
        {
            // if principal already in cookie
            if (wc.Cookies.TryGetValue("Token", out var token))
            {
                wc.Principal = Decrypt(token);
                return true;
            }

            // resolve principal thru OAuth2 or HTTP-basic
            User prin = null;
            string state = wc.Query[nameof(state)];
            if (WeiXin.WXAUTH.Equals(state)) // if weixin auth
            {
                string code = wc.Query[nameof(code)];
                if (code == null)
                {
                    return false;
                }
                (_, string openid) = await WeiXin.GetAccessorAsync(code);

                INF("openid = " + openid);
                if (openid == null)
                {
                    return false;
                }
                // check in db
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE wx = @1", p => p.Set(openid)))
                    {
                        prin = dc.ToObject<User>(0xff ^ User.PRIVACY);
                    }
                    else
                    {
                        prin = new User {wx = openid}; // create a minimal principal object
                    }
                }
            }
            else
            {
                string h_auth = wc.Header("Authorization");
                if (h_auth == null || !h_auth.StartsWith("Basic "))
                {
                    return true;
                }
                // decode basic scheme
                byte[] bytes = Convert.FromBase64String(h_auth.Substring(6));
                string orig = Encoding.ASCII.GetString(bytes);
                int colon = orig.IndexOf(':');
                string tel = orig.Substring(0, colon);
                string credential = TextUtility.MD5(orig);
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT * FROM users WHERE tel = @1", p => p.Set(tel)))
                    {
                        prin = dc.ToObject<User>();
                    }
                }
                // validate
                if (prin == null || !credential.Equals(prin.credential))
                {
                    return true;
                }
            }
            // setup principal and cookie
            if (prin != null)
            {
                // set token success
                wc.Principal = prin;
                wc.SetTokenCookie(prin, 0xff ^ User.PRIVACY);
            }
            return true;
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
                    dc.Execute("UPDATE users SET name = @1, tel = @2, grpat = @3, addr = @4 WHERE id = @5", p => p.Set(o.name).Set(o.tel).Set(o.teamat).Set(o.addr).Set(o.id));
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
                        WeiXin.GiveRedirectWeiXinAuthorize(wc, NETADDR);
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
                        h.LI_().SELECT("参　　团", nameof(o.teamat), o.teamat, orgs, tip: "（无）")._LI();
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

        [UserAccess(false)]
        public void @default(WebContext wc)
        {
            var arr = Obtain<Map<string, Item>>().All();
            wc.GivePage(200, h =>
                {
                    h.TOPBAR(true);
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
                }, true, 60
            );
        }

        public async Task onmsg(WebContext wc)
        {
            // wechat URL verification
            string echostr = wc.Query[nameof(echostr)];
            if (echostr != null)
            {
                wc.Give(200, echostr);
                return;
            }

            // event handling
            XElem xe = await wc.ReadAsync<XElem>();
            string MsgType = xe.Child(nameof(MsgType));
            if (MsgType == "event")
            {
                string Event = xe.Child(nameof(Event));
                if (Event == "subscribe") // SUBSCRIBE
                {
                    string EventKey = xe.Child(nameof(EventKey));
                    string FromUserName = xe.Child(nameof(FromUserName)); // wechat openid
                    string ToUserName = xe.Child(nameof(ToUserName));
                    using (var dc = NewDbContext())
                    {
                        if (EventKey.StartsWith("qrscene_"))
                        {
                            // make me same group as the referal
                            int refid = EventKey.ToInt(start: 8);
                            var grpat = (string) dc.Scalar("SELECT grpat FROM users WHERE id = @1", p => p.Set(refid));
                            dc.Execute("INSERT INTO users (wx, refid, grpat) VALUES (@1, @2, #3) ON CONFLICT (wx) DO NOTHING", p => p.Set(FromUserName).Set(refid).Set(grpat));
                        }
                        else
                        {
                            dc.Execute("INSERT INTO users (wx) VALUES (@1) ON CONFLICT (wx) DO NOTHING", p => p.Set(FromUserName));
                        }
                    }
                    // return msg
                    XmlContent x = new XmlContent(true, 1024);
                    x.ELEM("xml", null, () =>
                    {
                        x.ELEM("ToUserName", FromUserName);
                        x.ELEM("FromUserName", ToUserName);
                        x.ELEM("CreateTime", WeiXin.NowMillis);
                        x.ELEM("MsgType", "text");
                        x.ELEM("Content", "饮食里面包含智慧！\n\n愿您享受原造食品那奇妙的滋养和医治之能！和我们一起来维护这个健康、新鲜、良心的食品供应圈。");
                    });
                    wc.Give(200, x);
                    return;
                }
            }
            wc.Give(200);
        }

        /// <summary>
        /// WCPay notify, without authentic context.
        /// </summary>
        public async Task onpay(WebContext wc)
        {
            XElem xe = await wc.ReadAsync<XElem>();
            if (!WeiXin.OnNotified(xe, out var trade_no, out var cash))
            {
                wc.Give(400);
                return;
            }
            var orderid = trade_no.ToInt();
            string grpid, uname, uaddr;
            // update order status
            using (var dc = NewDbContext())
            {
                if (!dc.Query1("UPDATE orders SET cash = @1, paid = localtimestamp, status = 1 WHERE id = @2 AND status = 0 RETURNING grpid, uname, uaddr", (p) => p.Set(cash).Set(orderid)))
                {
                    return; // WCPay may send notification more than once
                }
                dc.Let(out grpid).Let(out uname).Let(out uaddr);
            }
            // send message to the related grouper, if any
            if (grpid != null)
            {
                var oprwx = Obtain<Map<string, Org>>()[grpid]?.mgrwx;
                if (oprwx != null)
                {
                    await WeiXin.PostSendAsync(oprwx, "新订单", ("¥" + cash + " " + uname + " " + uaddr), NETADDR + "/grp//ord/");
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