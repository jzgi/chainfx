using System;
using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;
using static Greatbone.Sample.Org;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    [Ui("设置"), User]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyVarVarWork, int>();

            Create<MyOrderWork>("order");
        }

        public void @default(WebContext ac)
        {
            var prin = (User) ac.Principal;
            ac.GivePage(200, m =>
            {
                m.TOOLBAR();
                m.BOARDVIEW(h =>
                    {
                        h.CARD_HEADER("账号信息");
                        h.CARD_BODY_();
                        h.FIELD(prin.name, "姓名");
                        h.FIELD(prin.tel, "电话");
                        h.FIELD_("地址").T(prin.city)._T(prin.addr)._FIELD();
                        h.BOX_();
                        h.QRCODE(GospelUtility.NETADDR + "/my//join?refwx=" + prin.wx, 3).P("让好友扫分享码，一同享用健康产品。");
                        h._BOX();
                        h._CARD_BODY();
                        h.CARD_FOOTER();
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

    public class MyVarVarWork : Work
    {
        public MyVarVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("修改"), Tool(ButtonShow)]
        public async Task edit(WebContext ac)
        {
            string wx = ac[-1];
            var prin = (User) ac.Principal;
            if (ac.GET)
            {
                if (ac.Query.Count > 0)
                {
                    ac.Query.Let(out prin.name).Let(out prin.tel).Let(out prin.city);
                }
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.TEXT(nameof(prin.name), prin.name, label: "姓名", max: 4, min: 2, required: true);
                    h.TEXT(nameof(prin.tel), prin.tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, required: true);
                    string city = prin.city ?? City.All[0].name;
                    h.SELECT(nameof(prin.city), city, City.All, "城市");
                    h.TEXT(nameof(prin.addr), prin.addr, label: "地址", max: 10, min: 2, required: true);
                    h._FORM();
                });
            }
            else
            {
                const byte proj = 0xff ^ CREDENTIAL ^ User.LATER;
                var o = await ac.ReadObjectAsync(obj: prin);
                o.wx = wx;
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO users")._(o, proj)._VALUES_(o, proj).T(" ON CONFLICT (wx) DO UPDATE")._SET_(o, proj ^ WX);
                    dc.Execute(p => o.Write(p, proj));
                }
                ac.SetTokenCookie(o, 0xff ^ CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        const string PASS = "0z4R4pX7";

        [Ui("设密码"), Tool(ButtonShow)]
        public async Task pass(WebContext ac)
        {
            User prin = (User) ac.Principal;
            string wx = ac[-1];
            string credential;
            string password = null;
            if (ac.GET)
            {
                using (var dc = NewDbContext())
                {
                    credential = (string) dc.Scalar("SELECT credential FROM users WHERE wx = @1", (p) => p.Set(wx));
                    if (credential != null)
                    {
                        password = PASS;
                    }
                    ac.GivePane(200, h =>
                    {
                        h.FORM_();
                        h.FIELDSET_("用于微信以外登录");
                        h.PASSWORD(nameof(password), password, label: "密码", max: 10, min: 3, required: true);
                        h._FIELDSET();
                        h._FORM();
                    });
                }
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                password = f[nameof(password)];
                if (password != PASS)
                {
                    credential = StrUtility.MD5(prin.tel + ":" + password);
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE users SET credential = @1 WHERE wx = @1", (p) => p.Set(credential).Set(wx));
                    }
                }
                ac.GivePane(200);
            }
        }
    }

    /// <summary>
    /// The working folder of org operators.
    /// </summary>
    [Ui("常规"), User(OPR)]
    public class OprVarWork : Work, IShopVar
    {
        public OprVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<OprNewoWork>("newo");

            Create<OprOldoWork>("oldo");

            Create<OprPosWork>("pos");

            Create<OprItemWork>("item");

            Create<OprOprWork>("opr");

            Create<OprCashWork>("cash");
        }

        public void @default(WebContext ac)
        {
            bool inner = ac.Query[nameof(inner)];
            if (!inner)
            {
                ac.GiveOffCanvas(200, false, 60 * 15, "内部操作");
                return;
            }

            var orgs = Obtain<Map<string, Org>>();
            string orgid = ac[this];
            ac.GivePage(200, h =>
            {
                h.TOOLBAR();
                var o = orgs[orgid];
                h.BOARDVIEW_();

                h.CARD_();
                h.CARD_HEADER(o.name, Statuses[o.status], o.status == 2);
                h.CARD_BODY_();
                h.FIELD(o.descr, "简介");
                h.FIELD_("限送").T(o.areas)._FIELD();
                h.FIELD_("活动").T(o.min).T("元起订，每满").T(o.notch).T("元立减").T(o.off).T("元")._FIELD();
                h.FIELD_("经理").T(o.mgrname)._T(o.mgrtel)._FIELD();
                h.FIELD_("客服").T(o.oprname)._T(o.oprtel)._FIELD();
                h._CARD_BODY();

                h._CARD();

                h._BOARDVIEW();
            });
        }

        static readonly string[] CRLF = {"\r\n", "\n"};

        [Ui("设置"), Tool(ButtonShow, 2), User(OPRMGR)]
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
                    h.TEXTAREA(nameof(o.descr), o.descr, "简介", max: 50, required: true);
                    h.TEXTAREA(nameof(o.areas), o.areas, "限送");
                    h.NUMBER(nameof(o.min), o.min, "起订", width: 2).NUMBER(nameof(o.notch), o.notch, "满额", width: 2).NUMBER(nameof(o.off), o.off, "立减", width: 2);
                    h._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                o.descr = f[nameof(o.descr)];
                string v = f[nameof(o.areas)];
                o.areas = v.Split(CRLF, StringSplitOptions.RemoveEmptyEntries);
                o.min = f[nameof(o.min)];
                o.notch = f[nameof(o.notch)];
                o.off = f[nameof(o.off)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orgs SET descr = @1, areas = @2, min = @3, notch = @4, off = @5 WHERE id = @6",
                        p => p.Set(o.descr).Set(o.areas).Set(o.min).Set(o.notch).Set(o.off).Set(orgid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("图示"), Tool(ButtonCrop, Ordinals = 3), User(OPRMGR)]
        public async Task img(WebContext wc, int ordinal)
        {
            string orgid = wc[this];
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    if (dc.Query1("SELECT img" + ordinal + " FROM orgs WHERE id = @1", p => p.Set(orgid)))
                    {
                        dc.Let(out ArraySegment<byte> byteas);
                        if (byteas.Count == 0) wc.Give(204); // no content 
                        else wc.Give(200, new StaticContent(byteas));
                    }
                    else wc.Give(404); // not found
                }
            }
            else // POST
            {
                var f = await wc.ReadAsync<Form>();
                ArraySegment<byte> jpeg = f[nameof(jpeg)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orgs SET img" + ordinal + " = @1 WHERE id = @2", p => p.Set(jpeg).Set(orgid));
                }
                wc.Give(200); // ok
            }
        }

        [Ui("上下班"), Tool(ButtonShow), User(OPRSTAFF)]
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