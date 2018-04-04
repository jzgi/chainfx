using System;
using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Core.Org;
using static Core.User;

namespace Core
{
    [Ui("设置"), User]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<MyOrderWork>("ord");
        }

        public void @default(WebContext ac)
        {
            var prin = (User) ac.Principal;
            ac.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.CARDVIEW(prin,
                    o => h.H4("账号信息"),
                    o =>
                    {
                        h.P(prin.name, "姓名");
                        h.P(prin.tel, "电话");
                        h.P_("地址")._T(prin.addr)._P();
                        h.COL_();
                        h.QRCODE(CoreUtility.NETADDR + "/my//join?refwx=" + prin.wx).P("让好友扫分享码，一同享用健康产品。");
                        h._COL();
                    }
                );
            });
        }

        public void join(WebContext wc)
        {
            string wx = wc[this];
            string refwx = wc.Query[nameof(refwx)];
            using (var dc = NewDbContext())
            {
                dc.Execute("INSERT INTO users (wx, refwx) VALUES (@1, @2) ON CONFLICT (wx) DO NOTHING", p => p.Set(wx).Set(refwx));
            }
            wc.GiveRedirect("https://mp.weixin.qq.com/mp/profile_ext?action=home&__biz=MzU4NDAxMTAwOQ==&scene=124#wechat_redirect");
        }

        const string VOIDPASS = "t#0^0z4R4pX7";

        [Ui("设置"), Tool(ButtonShow)]
        public async Task edit(WebContext wc)
        {
            string wx = wc[this];
            var prin = (User) wc.Principal;
            string password = null;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE wx = @1");
                    var o = dc.Query1<User>(p => p.Set(wx));
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
                        string credential = StrUtility.MD5(prin.tel + ":" + password);
                        dc.Execute(
                            "INSERT INTO users (wx, name, tel, credential) VALUES (@1, @2, @3, @4) ON CONFLICT (wx) DO UPDATE SET name = @2, tel = @3, credential = @4",
                            p => p.Set(prin.wx).Set(name).Set(tel).Set(credential)
                        );
                    }
                    else // password no change
                    {
                        dc.Execute(
                            "INSERT INTO users (wx, name, tel) VALUES (@1, @2, @3) ON CONFLICT (wx) DO UPDATE SET name = @2, tel = @3",
                            p => p.Set(prin.wx).Set(name).Set(tel)
                        );
                    }
                    prin.name = name;
                    prin.tel = tel;
                    wc.SetTokenCookie(prin, 0xff ^ CREDENTIAL);
                    wc.GivePane(200); // close dialog
                }
            }
        }

        [Ui("身份刷新"), Tool(ButtonOpen)]
        public void token(WebContext ac)
        {
            string wx = ac[this];
            using (var dc = NewDbContext())
            {
                const byte proj = 0xff ^ CREDENTIAL;
                if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(wx)))
                {
                    var o = dc.ToObject<User>(proj);
                    ac.SetTokenCookie(o, proj);
                    ac.GivePane(200);
                }
                else
                {
                    ac.GivePane(404);
                }
            }
        }
    }

    /// <summary>
    /// The working folder of org operators.
    /// </summary>
    [Ui("常规"), User(OPR)]
    public class OprVarWork : Work, IOrgVar
    {
        public OprVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<OprNewoWork>("newo");

            Create<OprOldoWork>("oldo");

            Create<OprItemWork>("item");

            Create<OprOprWork>("opr");

            Create<OprCashWork>("cash");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveOffCanvas(200, false, 60 * 15, "内部操作");
                return;
            }

            var orgs = Obtain<Map<string, Org>>();
            string orgid = wc[this];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                var org = orgs[orgid];

                h.CARDVIEW(org,
                    o => { h.H4(o.name); },
                    o =>
                    {
                        h.P(org.descr, "简介");
                        h.P_("经理").T(org.mgrname)._T(org.mgrtel)._P();
                        h.P_("客服").T(org.oprname)._T(org.oprtel)._P();
                    });
            });
        }

        static readonly string[] CRLF = {"\r\n", "\n"};

        [Ui("设置"), Tool(ButtonShow), User(OPRMGR)]
        public async Task sets(WebContext ac)
        {
            var orgs = Obtain<Map<string, Org>>();
            string orgid = ac[this];
            var o = orgs[orgid];
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("填写网点信息").TEXTAREA(nameof(o.descr), o.descr, "简介", max: 50, required: true)._FIELDSET();
                    h._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                o.descr = f[nameof(o.descr)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orgs SET descr = @1 WHERE id = @2",
                        p => p.Set(o.descr).Set(orgid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("上下班"), Tool(ButtonShow), User(OPRMEM)]
        public async Task status(WebContext ac)
        {
            var orgs = Obtain<Map<string, Org>>();
            User prin = (User) ac.Principal;
            string orgid = ac[this];
            var o = orgs[orgid];
            bool custsvc;
            if (ac.GET)
            {
                custsvc = o.oprwx == prin.wx;
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("设置网点营业状态").SELECT(nameof(o.status), o.status, Statuses, "营业状态")._FIELDSET();
                    h.FIELDSET_("客服设置");
                    h.CHECKBOX(nameof(custsvc), custsvc, "我要作为上线客服");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                o.status = f[nameof(o.status)];
                custsvc = f[nameof(custsvc)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orgs SET status = @1 WHERE id = @2", p => p.Set(o.status).Set(orgid));
                    if (custsvc)
                    {
                        dc.Execute("UPDATE orgs SET oprwx = @1, oprtel = @2, oprname = @3 WHERE id = @4", p => p.Set(prin.wx).Set(prin.tel).Set(prin.name).Set(orgid));
                        o.oprwx = prin.wx;
                        o.oprtel = prin.tel;
                        o.oprname = prin.name;
                    }
                    else
                    {
                        dc.Execute("UPDATE orgs SET oprwx = NULL, oprtel = NULL, oprname = NULL WHERE id = @1", p => p.Set(orgid));
                        o.oprwx = null;
                        o.oprtel = null;
                        o.oprname = null;
                    }
                }
                ac.GivePane(200);
            }
        }
    }
}