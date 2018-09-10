using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    [UserAccess(true)]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<MyOrderWork>("ord");

            Create<SampChatWork>("chat");
        }

        public void @default(WebContext wc)
        {
            var o = (User) wc.Principal;
            var orgs = Obtain<Map<string, Org>>();
            wc.GivePage(200, h =>
            {
                h.DIV_(css: "uk-card- uk-card-primary");
                h.UL_(css: "uk-card-body");
                h.LI_().FI("用户名称", o.name)._LI();
                h.LI_().FI("手　　机", o.tel)._LI();
                h.LI_().FI("参　　团", orgs[o.teamat]?.name)._LI();
                h.LI_().FI("收货地址", o.addr)._LI();
                h._UL();
                h.TOOLPAD(css: "uk-card-footer uk-flex-center");
                h._DIV();
            }, title: "我的设置");
        }

        const string PASSMASK = "t#0^0z4R4pX7";

        [Ui("填写设置", "填写我的设置"), Tool(ButtonShow, css: "uk-button-secondary")]
        public async Task edit(WebContext wc)
        {
            var prin = (User) wc.Principal;
            string password = null;
            if (wc.GET)
            {
                var orgs = Obtain<Map<string, Org>>();
                using (var dc = NewDbContext())
                {
                    var o = dc.Query1<User>("SELECT * FROM users WHERE id = @1", p => p.Set(prin.id));
                    if (o.credential != null) password = PASSMASK;
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDUL_("用户基本信息");
                        h.LI_().TEXT("用户名称", nameof(o.name), o.name, max: 4, min: 2, required: true)._LI();
                        h.LI_().TEXT("手　　机", nameof(o.tel), prin.tel, pattern: "[0-9]+", max: 11, min: 11, required: true)._LI();
                        h.LI_().SELECT("参　　团", nameof(o.teamat), prin.teamat, orgs, tip: "（无）")._LI();
                        h.LI_().TEXT("收货地址", nameof(o.addr), prin.addr, max: 20, min: 2, required: true)._LI();
                        h._FIELDUL();
                        h.FIELDUL_("用于从微信以外登录（可选）");
                        h.LI_().PASSWORD("外部密码", nameof(password), password, max: 12, min: 3)._LI();
                        h._FIELDUL();
                        h._FORM();
                    });
                }
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                prin.Read(f);
                password = f[nameof(password)];
                using (var dc = NewDbContext())
                {
                    if (password != PASSMASK) // password being changed
                    {
                        string credential = TextUtility.MD5(prin.tel + ":" + password);
                        dc.Execute("UPDATE users SET name = @1, tel = @2, grpat = @3, addr = @4, credential = @5 WHERE id = @6", p => p.Set(prin.name).Set(prin.tel).Set(prin.teamat).Set(prin.addr).Set(credential).Set(prin.id));
                    }
                    else // password no change
                    {
                        dc.Execute("UPDATE users SET name = @1, tel = @2, grpat = @3, addr = @4 WHERE id = @5", p => p.Set(prin.name).Set(prin.tel).Set(prin.teamat).Set(prin.addr).Set(prin.id));
                    }
                    wc.SetTokenCookie(prin, 0xff ^ PRIVACY);
                    wc.GivePane(200); // close dialog
                }
            }
        }

        [Ui("推荐他人", "推荐扫码关注"), Tool(AOpen, size: 1, css: "uk-button-secondary")]
        public async Task share(WebContext wc)
        {
            var prin = (User) wc.Principal;
            var (ticket, url) = await ((SampService) Service).WeiXin.PostQrSceneAsync(prin.id);
            wc.GivePage(200, h =>
            {
                h.DIV_("uk-padding uk-align-center uk-width-3-4");
                h.ICO_(circle: false).T("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=").T(ticket)._ICO();
                h._DIV();
            });
        }
    }


    [UserAccess(shop: 1)]
    [Ui("动态")]
    public class ShopVarWork : Work, IOrgVar
    {
        public ShopVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<ShopOrderWork>("order");

            Create<OrgRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[this];
            var shop = Obtain<Map<string, Org>>()[orgid];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 900, shop?.name);
            }
            else
            {
                wc.GivePage(200, h => { h.TOOLBAR(); });
            }
        }
    }

    [UserAccess(team: 1)]
    [Ui("概况")]
    public class TeamVarWork : Work, IOrgVar
    {
        public TeamVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<TeamOrderWork>("order");

            Create<TeamUserWork>("user");

            Create<OrgRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            string teamid = wc[this];
            var team = Obtain<Map<string, Org>>()[teamid];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 900, team?.name);
            }
            else
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();

                    h.SECTION_("uk-card uk-card-primary");
                    h.HEADER_("uk-card-header").H4("团组")._HEADER();
                    h.MAIN_("uk-card-body")._MAIN();
                    h._SECTION();

                    h.SECTION_("uk-card uk-card-primary");
                    h.HEADER_("uk-card-header").H4("订单")._HEADER();
                    h.MAIN_("uk-card-body")._MAIN();
                    h._SECTION();
                });
            }
        }

        [Ui("发通知"), Tool(ButtonPickShow)]
        public void send(WebContext wc)
        {
            long[] key = wc.Query[nameof(key)];
            string msg = null;
            if (wc.GET)
            {
                wc.GivePane(200, m =>
                {
                    m.FORM_();
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
                wc.GivePane(200);
            }
        }
    }
}