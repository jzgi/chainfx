using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Org;
using static Samp.User;

namespace Samp
{
    [User]
    [Ui("设置")]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<MyOrderWork>("ord");

            Create<MyChatWork>("chat");
        }

        public void @default(WebContext wc)
        {
            var prin = (User) wc.Principal;
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.CARDVIEW(prin, null,
                    o =>
                    {
                        h.P(prin.name, "姓名");
                        h.P(prin.tel, "电话");
                        h.P_("地址")._T(prin.addr)._P();
                        h.COL_();
                        h.QRCODE(SampUtility.NETADDR + "/my//join?refwx=" + prin.wx).P("让好友扫分享码，一同享用健康产品。");
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
            var prin = (User) wc.Principal;
            string password = null;
            if (wc.GET)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE id = @1");
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
                        string credential = StrUtility.MD5(prin.tel + ":" + password);
                        dc.Execute("UPDATE users SET name = @1, tel = @2, credential = @3 WHERE id = @4", p => p.Set(name).Set(tel).Set(credential).Set(prin.id));
                    }
                    else // password no change
                    {
                        dc.Execute("UPDATE users SET name = @1, tel = @2 WHERE id = @3", p => p.Set(name).Set(tel).Set(prin.id));
                    }
                    prin.name = name;
                    prin.tel = tel;
                    wc.SetTokenCookie(prin, 0xff ^ CREDENTIAL);
                    wc.GivePane(200); // close dialog
                }
            }
        }

        [Ui("重新登录"), Tool(ButtonScript)]
        public void rmtoken(WebContext wc)
        {
        }
    }

    /// <summary>
    /// The working folder of org operators.
    /// </summary>
    [User(opr: 1)]
    [Ui("常规")]
    public class OprVarWork : Work, IOrgVar
    {
        public OprVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<OprChatWork>("chat");

            Create<OprNewoWork>("newo");

            Create<OprOldoWork>("oldo");

            Create<OprItemWork>("item");

            Create<OprCashWork>("cash");
        }

        public void @default(WebContext wc)
        {
            var orgs = Obtain<Map<string, Org>>();
            string orgid = wc[this];
            var org = orgs[orgid];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 60 * 15, org?.name);
                return;
            }

            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                h.CARDVIEW(org, null,
                    o =>
                    {
                        h.P(org.descr, "简介");
                        h.P_("经理").T(org.mgrname)._T(org.mgrtel)._P();
                        h.P_("客服").T(org.oprname)._T(org.oprtel)._P();
                    });
            });
        }

        [Ui("人员"), Tool(ButtonOpen), User(OPRMGR)]
        public async Task access(WebContext wc, int cmd)
        {
            string orgid = wc[this];
            string tel = null;
            short opr = 0;
            var f = await wc.ReadAsync<Form>();
            if (f != null)
            {
                tel = f[nameof(tel)];
                opr = f[nameof(opr)];
                if (cmd == 1) // remove
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = NULL, opr = 0 WHERE tel = @1", p => p.Set(tel));
                    }
                }
                else if (cmd == 2) // add
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE tel = @3", p => p.Set(orgid).Set(opr).Set(tel));
                    }
                }
            }
            wc.GivePane(200, h =>
            {
                h.FORM_();
                h.FIELDSET_("现有人员");
                using (var dc = NewDbContext())
                {
                    if (dc.Query("SELECT name, tel, opr FROM users WHERE oprat = @1", p => p.Set(orgid)))
                    {
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out tel).Let(out opr);
                            h.RADIO(nameof(tel), tel, tel + " " + name + " " + Oprs[opr], false);
                        }
                    }
                }
                h._FIELDSET();
                h.BUTTON(nameof(access), 1, "删除");

                h.FIELDSET_("添加人员");
                h.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11);
                h.SELECT(nameof(opr), opr, Oprs, "角色");
                h._FIELDSET();
                h.BUTTON(nameof(access), 2, "添加");
                h._FORM();
            });
        }

        [Ui("状态"), Tool(ButtonShow), User(OPRMEM)]
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