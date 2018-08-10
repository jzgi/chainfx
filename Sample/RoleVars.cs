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
            wc.GivePage(200, h =>
            {
                h.T("<section class=\"uk-card- uk-card-primary \">");
                h.UL_("uk-card-body");
                h.LI_().FI("用户名称", o.name)._LI();
                h.LI_().FI("手　　机", o.tel)._LI();
                h.LI_().FI("收货地址", o.addr)._LI();
                h.LI_().FI("积　　分", o.score)._LI();
                h._UL();
                h.TOOLS(css: "uk-card-footer");
                h.T("</section>");
            }, title: "我的设置");
        }

        const string VOIDPASS = "t#0^0z4R4pX7";

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(WebContext wc)
        {
            var prin = (User) wc.Principal;
            string password = null;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE id = @1");
                    var o = dc.Query1<User>(p => p.Set(prin.id));
                    if (o.credential != null) password = VOIDPASS;
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("用户基本信息（必填）");
                        h.TEXT(nameof(o.name), o.name, label: "姓名", max: 4, min: 2, required: true);
                        h.TEXT(nameof(o.tel), prin.tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, required: true);
                        h._FIELDSET();
                        h.FIELDSET_("用于从APP登录（可选）");
                        h.PASSWORD(nameof(password), password, label: "密码", max: 12, min: 3);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                string name = f[nameof(name)];
                string tel = f[nameof(tel)];
                password = f[nameof(password)];
                using (var dc = NewDbContext())
                {
                    if (password != VOIDPASS) // password being changed
                    {
                        string credential = TextUtility.MD5(prin.tel + ":" + password);
                        dc.Execute("UPDATE users SET name = @1, tel = @2, credential = @3 WHERE id = @4", p => p.Set(name).Set(tel).Set(credential).Set(prin.id));
                    }
                    else // password no change
                    {
                        dc.Execute("UPDATE users SET name = @1, tel = @2 WHERE id = @3", p => p.Set(name).Set(tel).Set(prin.id));
                    }
                    prin.name = name;
                    prin.tel = tel;
                    wc.SetTokenCookie(prin, 0xff ^ PRIVACY);
                    wc.GivePane(200); // close dialog
                }
            }
        }

        [Ui("设密码", "设外部登录密码"), Tool(ButtonShow)]
        public async Task pass(WebContext wc)
        {
            var prin = (User) wc.Principal;
            string password = null;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT credential FROM users WHERE id = @1");
                    var credential = (string) dc.Scalar("SELECT credential FROM users WHERE id = @1", p => p.Set(prin.id));
                    if (credential != null)
                    {
                        password = VOIDPASS;
                    }
                    wc.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("用于从微信以外登录（可选）");
                        h.PASSWORD(nameof(password), password, label: "密码", max: 12, min: 3);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                password = f[nameof(password)];
                using (var dc = NewDbContext())
                {
                    if (password != VOIDPASS) // password being changed
                    {
                        string credential = TextUtility.MD5(prin.tel + ":" + password);
                        dc.Execute("UPDATE users SET credential = @1 WHERE id = @2", p => p.Set(credential).Set(prin.id));
                    }
                    wc.GivePane(200); // close dialog
                }
            }
        }

        [Ui("推荐给", "让好友扫码关注全粮派"), Tool(AOpen)]
        public async Task share(WebContext wc)
        {
            var prin = (User) wc.Principal;
            var (ticket, url) = await ((SampService) Service).WeiXin.PostQrSceneAsync(prin.id);
            wc.GivePage(200, h =>
            {
                h.T("<div class=\"uk-padding uk-align-center uk-width-3-4\">");
                h.ICO_().T("https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=").T(ticket)._ICO();
                h.T("</div>");
            });
        }
    }


    [UserAccess(grp: 1)]
    [Ui("团组")]
    public class GrpVarWork : UserWork<GrpUserVarWork>, IOrgVar
    {
        public GrpVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<GrpOrderWork>("ord");
        }

        public void @default(WebContext wc)
        {
            string grpid = wc[this];
            var team = Obtain<Map<string, Org>>()[grpid];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 60 * 15, team?.name);
            }
            else
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE grpat = @1 ORDER BY id");
                        var arr = dc.Query<User>(p => p.Set(grpid));
                        h.TABLE(arr, null,
                            o => h.TD(o.name).TD(o.tel).TD(o.addr)
                        );
                    }
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