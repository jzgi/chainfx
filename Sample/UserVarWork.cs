using System;
using System.Threading.Tasks;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }
    }

    [Ui("设置")]
    public class MyUserVarWork : UserVarWork
    {
        public MyUserVarWork(WorkContext wc) : base(wc)
        {
            Create<MyCartOrderWork>("cart");

            Create<MyActiveOrderWork>("active");

            Create<MyPastOrderWork>("past");

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
                    m.TEXT(nameof(prin.id), prin.id, "用户编号（个人手机号）", max: 11, min: 11, pattern: "[0-9]+", required: true);
                    m.PASSWORD(nameof(password), password, "登录密码（用于微信以外登录）", min: 3);
                    m.SELECT(nameof(prin.city), prin.city, ((ShopService) Service).CityOpt, label: "城市");
                    m._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                prin.id = f[nameof(prin.id)];
                prin.name = f[nameof(prin.name)];
                password = f[nameof(password)];
                prin.city = f[nameof(prin.city)];
                using (var dc = ac.NewDbContext())
                {
                    if (PASS == password)
                    {
                        dc.Execute("INSERT INTO users (wx, id, name, city) VALUES (@1, @2, @3, @4) ON CONFLICT (wx) DO UPDATE SET id = @2, name = @3, city = @4 ", p => p.Set(wx).Set(prin.id).Set(prin.name).Set(prin.city));
                    }
                    else
                    {
                        string credential = StrUtility.MD5(prin.id + ":" + password);
                        dc.Execute("INSERT INTO users (wx, id, name, credential, city) VALUES (@1, @2, @3, @4, @5) ON CONFLICT (wx) DO UPDATE SET id = @2, name = @3, credential = @4, city = @5", p => p.Set(wx).Set(prin.id).Set(prin.name).Set(credential).Set(prin.city));
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

    public class OprUserVarWork : UserVarWork
    {
        public OprUserVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class AdmUserVarWork : UserVarWork
    {
        public AdmUserVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}