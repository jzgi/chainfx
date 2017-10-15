using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.UiMode;
using static Greatbone.Sample.User;

namespace Greatbone.Sample
{
    [Ui("常规"), User]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkContext wc) : base(wc)
        {
            Create<MyPreWork>("pre");

            Create<MyOrderWork>("order");

            Create<MyKickWork>("kick");

            CreateVar<MyVarVarWork, int>();
        }

        public void @default(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User) ac.Principal;
            ac.GivePage(200, main =>
            {
                main.GridView(h =>
                {
                    h.CAPTION("个人资料");
                    h.FIELD("姓名", prin.name);
                    h.FIELD("密码", null);
                    h.FIELD("城市", prin.city);
                    h.FIELDSET_("默认收货地址");
                    h.FIELD("地址", prin.addr);
                    h._FIELDSET();
                });
            });
        }

        [Ui("刷新", Mode = AShow)]
        public void token(ActionContext ac)
        {
            string wx = ac[this];
            using (var dc = ac.NewDbContext())
            {
                const short proj = -1 ^ CREDENTIAL;
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
        const string PASS = "0z4R4pX7";

        public MyVarVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("修改", Mode = ButtonShow)]
        public async Task profile(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User) ac.Principal;
            string password = PASS;
            if (ac.GET)
            {
                ac.GivePage(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("后台操作人员用本人的微信填写");
                    h.TEXT(nameof(prin.name), prin.name, "真实姓名（和身份证一致）", max: 4, min: 2, required: true);
                    h.TEXT(nameof(prin.tel), prin.tel, "用户编号（个人手机号）", pattern: "[0-9]+", max: 11, min: 11, required: true);
                    h.PASSWORD(nameof(password), password, "登录密码（用于微信以外登录）", min: 3);
                    h.SELECT(nameof(prin.city), prin.city, ((SampleService) Service).Cities, label: "城市");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                prin.tel = f[nameof(prin.tel)];
                prin.name = f[nameof(prin.name)];
                password = f[nameof(password)];
                prin.city = f[nameof(prin.city)];
                using (var dc = ac.NewDbContext())
                {
                    if (PASS == password)
                    {
                        dc.Execute("INSERT INTO users (wx, id, name, city) VALUES (@1, @2, @3, @4) ON CONFLICT (wx) DO UPDATE SET id = @2, name = @3, city = @4 ", p => p.Set(wx).Set(prin.tel).Set(prin.name).Set(prin.city));
                    }
                    else
                    {
                        string credential = StrUtility.MD5(prin.tel + ":" + password);
                        dc.Execute("INSERT INTO users (wx, id, name, credential, city) VALUES (@1, @2, @3, @4, @5) ON CONFLICT (wx) DO UPDATE SET id = @2, name = @3, credential = @4, city = @5", p => p.Set(wx).Set(prin.tel).Set(prin.name).Set(credential).Set(prin.city));
                    }
                }
                ac.SetTokenCookie(prin, -1 ^ CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("设密码", Mode = AShow)]
        public void password(ActionContext ac)
        {
            string wx = ac[this];
            using (var dc = ac.NewDbContext())
            {
                const short proj = -1 ^ CREDENTIAL;
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


    [Ui("常规")]
    [User(OPR_)]
    public class OprVarWork : Work
    {
        public OprVarWork(WorkContext wc) : base(wc)
        {
            Create<OprNewWork>("new");

            Create<OprGoWork>("on");

            Create<OprPastWork>("past");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");

            CreateVar<OprVarVarWork, int>();
        }

        public void @default(ActionContext ac)
        {
            bool inner = ac.Query[nameof(inner)];
            if (inner)
            {
                short shopid = ac[this];
                ac.GivePage(200, main =>
                {
                    main.GridView(h =>
                    {
                        using (var dc = ac.NewDbContext())
                        {
                            dc.Query1("SELECT oprwx, oprtel, oprname, status FROM shops WHERE id = @1", p => p.Set(shopid));
                            dc.Let(out string oprwx).Let(out string oprtel).Let(out string oprname).Let(out short status);
                            h.CAPTION("营业状态设置");
                            h.FIELD(status, "状态", opt: Shop.STATUS);
                            h.FIELDSET_("值班员");
                            h.FIELD(oprname, "姓名");
                            h.FIELD(oprwx, "微信");
                            h.FIELD(oprtel, "电话");
                            h._FIELDSET();
                        }
                    });
                });
            }
            else
            {
                Node node = ac[this];
                ac.GiveFrame(200, false, 60 * 15, node.Label);
            }
        }

        [Ui("操作授权", Mode = AOpen), User(OPRMEM)]
        public async Task grant(ActionContext ac, int cmd)
        {
            short shopid = ac[this];
            string tel = null;
            short opr = 0;
            var f = await ac.ReadAsync<Form>();
            if (f != null)
            {
                tel = f[nameof(tel)];
                opr = f[nameof(opr)];
                if (cmd == 1) // remove
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = NULL, opr = 0 WHERE tel = @1", p => p.Set(tel));
                    }
                }
                else if (cmd == 2) // add
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE tel = @3", p => p.Set(shopid).Set(opr).Set(tel));
                    }
                }
            }
            ac.GivePane(200, m =>
            {
                m.FORM_();
                m.FIELDSET_("现有操作人员");
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT name, tel, opr FROM users WHERE oprat = @1", p => p.Set(shopid)))
                    {
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out tel).Let(out opr);
                            m.RADIO(nameof(tel), tel, null, null, false, tel, name, OPRS[opr]);
                        }
                        m.BUTTON(nameof(grant), 1, "删除");
                    }
                }
                m._FIELDSET();

                m.FIELDSET_("添加操作人员");
                m.TEXT(nameof(tel), tel, "手机号", pattern: "[0-9]+", max: 11, min: 11);
                m.SELECT(nameof(opr), opr, OPRS, "权限");
                m.BUTTON(nameof(grant), 2, "添加");
                m._FIELDSET();
                m._FORM();
            });
        }

        public void poll(ActionContext ac)
        {
            short shopid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                int c = (int) dc.Scalar("SELECT count(*) FROM orders WHERE shopid = @1 AND status = 1", p => p.Set(shopid));
                StrContent str = new StrContent(true, false);
                str.Add("本作坊有");
                str.Add(c);
                str.Add("个未处理订单");
                ac.Give(200, str);
            }
        }
    }

    public class OprVarVarWork : Work
    {
        public OprVarVarWork(WorkContext wc) : base(wc)
        {
        }

        [Ui("设下班", Mode = ButtonShow)]
        public void setoff(ActionContext ac)
        {
            short shopid = ac[-1];
            bool yes = false;
            User prin = (User) ac.Principal;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.CHECKBOX(nameof(yes), yes, "确认下班吗？系统将停止接单", required: true);
                    h._FORM();
                });
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET oprwx = NULL, oprtel = NULL, oprname = NULL, status = " + Shop.OFF + " WHERE id = @1", p => p.Set(shopid));
                }
                ac.GivePane(200);
            }
        }

        [Ui("我值班", Mode = ButtonShow)]
        public void seton(ActionContext ac)
        {
            short shopid = ac[-1];
            bool yes = false;
            User prin = (User) ac.Principal;
            if (ac.GET)
            {
                ac.GivePane(200, h =>
                {
                    h.FORM_();
                    h.CHECKBOX(nameof(yes), yes, "确认将本用户设为值班员", required: true);
                    h._FORM();
                });
            }
            else
            {
                using (var dc = ac.NewDbContext())
                {
                    dc.Execute("UPDATE shops SET oprwx = @1, oprtel = @2, oprname = @3, status = " + Shop.ON + " WHERE id = @4", p => p.Set(prin.wx).Set(prin.tel).Set(prin.name).Set(shopid));
                }
                ac.GivePane(200);
            }
        }
    }
}