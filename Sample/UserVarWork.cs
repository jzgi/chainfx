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

            Create<MyChargeWork>("tipoff");
        }

        [Ui("基本信息", Mode = UiMode.AnchorShow)]
        public async Task profile(ActionContext ac)
        {
            string wx = ac[this];
            if (ac.GET)
            {
                var o = ac.Query.ToData<User>();
                if (o.nickname == null)
                {
                    User prin = (User) ac.Principal;
                    o.nickname = prin.nickname;
                    o.city = prin.city;
                    o.addr = prin.addr;
                    o.distr = prin.distr;
                    o.tel = prin.tel;
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.FIELDSET_("默认收货地址");
                    m.TEXT(nameof(o.nickname), o.nickname, label: "用户名称", max: 10, required: true);
                    m.SELECT(nameof(o.city), o.city, ((ShopService) Service).CityOpt, label: "城市", refresh: true);
                    m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), label: "区划");
                    m.TEXT(nameof(o.addr), o.addr, label: "街道/地址");
                    m.TEXT(nameof(o.tel), o.tel, label: "联系电话");
                    m._FIELDSET();
                    m._FORM();
                });
            }
            else
            {
                var o = await ac.ReadDataAsync<User>();
                o.wx = wx;
                o.created = DateTime.Now;
                using (var dc = ac.NewDbContext())
                {
                    const ushort proj = User.WX | User.CREATTED;
                    dc.Sql("INSERT INTO users ")._(User.Empty, proj)._VALUES_(User.Empty, proj)._("ON CONFLICT (wx) DO UPDATE")._SET_(User.Empty);
                    dc.Execute(p => o.WriteData(p, proj));
                }

                ac.SetTokenCookie(o, 0xffff ^ User.CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
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