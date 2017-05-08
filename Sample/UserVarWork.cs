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

            Create<MyPresentOrderWork>("current");

            Create<MyPastOrderWork>("history");
        }

        [Ui("个人资料", Mode = UiMode.AnchorDialog)]
        public void profile(ActionContext ac)
        {
            string userid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT * FROM users WHERE id = @1", p => p.Set(userid)))
                {
                    var user = dc.ToObject<User>();
                    ac.GivePane(200, null);
                }
                else
                {
                    ac.Give(404); // not found
                }
            }
        }

        [Ui("登录号密码", Mode = UiMode.AnchorDialog)]
        public void loginf(ActionContext ac)
        {
            string id = null;
            string password = null;
            if (ac.GET)
            {
                ac.GivePane(200, (x) =>
                {
                    x.FORM_();
                    x.TEXT(nameof(id), id, label: "个人手机号", max: 11, min: 11, pattern: "[0-9]", required: true);
                    x.PASSWORD(nameof(password), password, label: " 登录密码", min: 3, required: true);
                    x._FORM();
                });
            }
            else
            {
            }
        }

        [Ui("经理商家", Mode = UiMode.AnchorDialog)]
        public async Task mgrof(ActionContext ac)
        {
            User prin = (User) ac.Principal;

            if (prin.id == null)
            {
                ac.GivePane(200, (x) => { x.CALLOUT("请先设置个人后台登录号及密码", false); });
                return;
            }

            string shopid = null;
            string password = null;
            if (ac.GET)
            {
                ac.GivePane(200, (x) =>
                {
                    x.FORM_();
                    x.TEXT(nameof(shopid), shopid, label: "商家编号", max: 6, min: 6, pattern: "[0-9]", required: true);
                    x.PASSWORD(nameof(password), password, label: "商家密码", min: 3, required: true);
                    x._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                shopid = f[nameof(shopid)];
                password = f[nameof(password)];

                // data op
                using (var dc = ac.NewDbContext())
                {
                    var credential = (string) dc.Scalar("SELECT credential FROM shops WHERE id = @1", p => p.Set(shopid));
                    if (credential.EqualsCredential(shopid, password))
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE wx = @3", p => p.Set(shopid).Set(User.MANAGER).Set(prin.wx));
                        prin.oprat = shopid;
                        ac.SetTokenCookie(prin, -1);
                    }
                }
                ac.GivePane(200, null);
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