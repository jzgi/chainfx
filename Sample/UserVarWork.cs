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

            Create<MyChargeWork>("charge");
        }

        const string PASS = "0z4R4pX7";

        [Ui("后台操作设置", "后台操作帐号", Mode = UiMode.AnchorShow)]
        public async Task loginf(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User)ac.Principal;
            string password = PASS;
            if (ac.GET)
            {
                ac.GivePane(200, (x) =>
                {
                    x.FORM_();
                    x.FIELDSET_("仅后台操作人员填写");
                    x.TEXT(nameof(prin.id), prin.id, "用户编号（个人手机号）", max: 11, min: 11, pattern: "[0-9]+", required: true);
                    x.TEXT(nameof(prin.name), prin.name, "真实姓名（和身份证一致）", max: 4, min: 2, required: true);
                    x.PASSWORD(nameof(password), password, "登录密码（用于微信以外登录）", min: 3);
                    x._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                prin.id = f[nameof(prin.id)];
                prin.name = f[nameof(prin.name)];
                password = f[nameof(password)];

                using (var dc = ac.NewDbContext())
                {
                    if (PASS == password)
                    {
                        dc.Execute("UPDATE users SET id = @1, name = @2 WHERE wx = @3", p => p.Set(prin.id).Set(prin.name).Set(wx));
                    }
                    else
                    {
                        string credential = StrUtility.MD5(prin.id + ":" + password);
                        dc.Execute("UPDATE users SET id = @1, name = @2, credential = @3 WHERE wx = @4", p => p.Set(prin.id).Set(prin.name).Set(credential).Set(wx));
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
                const ushort proj = 0xffff ^ User.CREDENTIAL;
                if (dc.Query1("SELECT * FROM users WHERE wx = @1", (p) => p.Set(wx)))
                {
                    var o = dc.ToData<User>(proj);
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