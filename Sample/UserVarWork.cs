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

            Create<MyPresentOrderWork>("present");

            Create<MyPastOrderWork>("past");
        }

        [Ui("个人资料", Mode = UiMode.AnchorDialog)]
        public async Task profile(ActionContext ac)
        {
            string wx = ac[this];
            if (ac.GET)
            {
                User prin = (User) ac.Principal;
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(prin.name), prin.name, label: "用户名称（后台人员须用实名）", max: 10, required: true);
                    m.FIELDSET_("默认收货地址");
                    m.SELECT(nameof(prin.city), prin.city, ((ShopService) Service).CityOpt, label: "城市", refresh: true);
                    m.SELECT(nameof(prin.distr), prin.distr, ((ShopService) Service).GetDistrs(prin.city), label: "区域");
                    m.TEXT(nameof(prin.addr), prin.addr, label: "地址");
                    m.TEXT(nameof(prin.tel), prin.tel, label: "联系电话");
                    m._FIELDSET();
                    m._FORM();
                });
            }
            else
            {
                const short proj = -1 ^ User.LOGIN ^ User.PERM;
                var user = await ac.ReadObjectAsync<User>();
                user.wx = wx;
                user.created = DateTime.Now;
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("INSERT INTO users ")._(User.Empty, proj)._VALUES_(User.Empty, proj)._("ON CONFLICT (wx) DO UPDATE")._SET_(User.Empty, proj ^ User.CREATED);
                    dc.Execute(p => user.WriteData(p, proj));
                }

                ac.SetTokenCookie(user, -1 ^ User.CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("后台标识信息", Mode = UiMode.AnchorDialog)]
        public async Task loginf(ActionContext ac)
        {
            string wx = ac[this];
            var prin = (User) ac.Principal;
            string password = null;
            if (ac.GET)
            {
                ac.GivePane(200, (x) =>
                {
                    x.FORM_();
                    x.TEXT(nameof(prin.id), prin.id, label: "个人手机号（不要轻易改变）", max: 11, min: 11, pattern: "[0-9]+", required: true);
                    x.PASSWORD(nameof(password), password, label: " 登录密码（用于在微信以外登录）", min: 3);
                    x._FORM();
                });
            }
            else
            {
                var f = await ac.ReadAsync<Form>();
                prin.id = f[nameof(prin.id)];
                password = f[nameof(password)];

                using (var dc = ac.NewDbContext())
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        dc.Execute("UPDATE users SET id = @1 WHERE wx = @2", p => p.Set(prin.id).Set(wx));
                    }
                    else
                    {
                        string credential = StrUtility.MD5(prin.id + ":" + password);
                        dc.Execute("UPDATE users SET id = @1, credential = @2 WHERE wx = @3", p => p.Set(prin.id).Set(credential).Set(wx));
                    }
                }

                ac.SetTokenCookie(prin, -1 ^ User.CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        [Ui("绑定到商家", Mode = UiMode.AnchorDialog)]
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