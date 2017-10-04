using System.Threading.Tasks;
using Greatbone.Core;

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
        }

        const string PASS = "0z4R4pX7";

        [Ui("后台操作设置", "后台操作帐号", Mode = UiMode.AnchorShow)]
        public async Task loginf(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User) ac.Principal;
            string password = PASS;
            if (ac.GET)
            {
                ac.GivePane(200, (m) =>
                {
                    m.FORM_();
                    m.FIELDSET_("后台操作人员用本人的微信填写");
                    m.TEXT(nameof(prin.name), prin.name, "真实姓名（和身份证一致）", max: 4, min: 2, required: true);
                    m.TEXT(nameof(prin.tel), prin.tel, "用户编号（个人手机号）", max: 11, min: 11, pattern: "[0-9]+", required: true);
                    m.PASSWORD(nameof(password), password, "登录密码（用于微信以外登录）", min: 3);
                    m.SELECT(nameof(prin.city), prin.city, ((SampleService) Service).Cities, label: "城市");
                    m._FORM();
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
                ac.SetTokenCookie(prin, 0xffff ^ User.CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("调试刷新", Mode = UiMode.AnchorShow)]
        public void token(ActionContext ac)
        {
            string wx = ac[this];
            using (var dc = ac.NewDbContext())
            {
                const int proj = 0xffff ^ User.CREDENTIAL;
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
    [User(User.OPRBASE)]
    public class OprVarWork : Work
    {
        public OprVarWork(WorkContext wc) : base(wc)
        {
            Create<OprNewOrderWork>("new");

            Create<OprOnOrderWork>("on");

            Create<OprPastOrderWork>("past");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200, false, 60 * 5);
        }

        [Ui("值班主管", Mode = UiMode.AnchorShow)]
        public async Task lead(ActionContext ac)
        {
            short shopid = ac[this];
            bool me = false;
            if (ac.GET)
            {
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query1("SELECT oprtel, oprname FROM shops WHERE id = @1", p => p.Set(shopid)))
                    {
                        dc.Let(out string oprtel).Let(out string oprname);
                        ac.GivePane(200, m =>
                        {
                            m.FORM_();
                            m.FIELDSET_("当前值班主管");
                            m.COL("电话", oprtel);
                            m.COL("姓名", oprname);
                            m._FIELDSET();
                            m.CHECKBOX(nameof(me), me, "把我设为值班主管");
                            m._FORM();
                        });
                    }
                    else ac.Give(404); // not found
                }
            }
            else // post
            {
                var f = await ac.ReadAsync<Form>();
                me = f[nameof(me)];
                User prin = (User) ac.Principal;
                if (me)
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Sql("UPDATE shops SET oprwx = @1, oprtel = @2, oprname = @3 WHERE id = @1");
                        dc.Execute("UPDATE shops SET oprwx = @1, oprtel = @2, oprname = @3 WHERE id = @4", p => p.Set(prin.wx).Set(prin.tel).Set(prin.name).Set(shopid));
                    }
                }
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("操作授权", Mode = UiMode.AnchorOpen)]
        public async Task crew(ActionContext ac, int subcmd)
        {
            short shopid = ac[this];

            // form submitted values
            string id;
            string name;
            string oprid = null;
            short opr = 0;

            var f = await ac.ReadAsync<Form>();
            if (f != null)
            {
                id = f[nameof(id)];
                oprid = f[nameof(oprid)];
                opr = f[nameof(opr)];
                if (subcmd == 1) // remove
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = NULL, opr = 0 WHERE id = @1", p => p.Set(id));
                    }
                }
                else if (subcmd == 2) // add
                {
                    using (var dc = ac.NewDbContext())
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE id = @3", p => p.Set(shopid).Set(opr).Set(oprid));
                    }
                }
            }

            ac.GivePane(200, m =>
            {
                m.FORM_();

                m.FIELDSET_("现有操作授权");
                using (var dc = ac.NewDbContext())
                {
                    if (dc.Query("SELECT id, name, opr FROM users WHERE oprat = @1", p => p.Set(shopid)))
                    {
                        while (dc.Next())
                        {
                            dc.Let(out id).Let(out name).Let(out opr);
                            m.RADIO(nameof(id), id, null, null, false, id, name, User.OPR[opr]);
                        }
                        m.BUTTON(nameof(crew), 1, "删除");
                    }
                }
                m._FIELDSET();

                m.FIELDSET_("添加操作授权");
                m.TEXT(nameof(oprid), oprid, "个人手机号", max: 11, min: 11, pattern: "[0-9]+");
                m.SELECT(nameof(opr), opr, User.OPR, "操作权限");
                m.BUTTON(nameof(crew), 2, "添加");
                m._FIELDSET();
                m._FORM();
            });
        }
    }
}