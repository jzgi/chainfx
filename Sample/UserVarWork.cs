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

        [Ui("基本信息", Mode = UiMode.AnchorDialog)]
        public async Task profile(ActionContext ac)
        {
            string wx = ac[this];
            if (ac.GET)
            {
                var o = ac.Query.ToObject<User>();
                if (o.name == null)
                {
                    User prin = (User) ac.Principal;
                    o.name = prin.name;
                    o.city = prin.city;
                    o.addr = prin.addr;
                    o.distr = prin.distr;
                    o.tel = prin.tel;
                }
                ac.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.name), o.name, label: "用户名称（后台人员须用实名）", max: 10, required: true);
                    m.FIELDSET_("默认收货地址");
                    m.SELECT(nameof(o.city), o.city, ((ShopService) Service).CityOpt, label: "城市", refresh: true);
                    m.SELECT(nameof(o.distr), o.distr, ((ShopService) Service).GetDistrs(o.city), label: "区域");
                    m.TEXT(nameof(o.addr), o.addr, label: "地址");
                    m.TEXT(nameof(o.tel), o.tel, label: "联系电话");
                    m._FIELDSET();
                    m._FORM();
                });
            }
            else
            {
                const short proj = -1 ^ User.LOGIN ^ User.PERM;
                var o = await ac.ReadObjectAsync<User>();
                o.wx = wx;
                o.created = DateTime.Now;
                using (var dc = ac.NewDbContext())
                {
                    dc.Sql("INSERT INTO users ")._(User.Empty, proj)._VALUES_(User.Empty, proj)._("ON CONFLICT (wx) DO UPDATE")._SET_(User.Empty, proj ^ User.CREATED);
                    dc.Execute(p => o.WriteData(p, proj));
                }

                ac.SetTokenCookie(o, -1 ^ User.CREDENTIAL);
                ac.GivePane(200); // close dialog
            }
        }

        const string PASS = "0z4R4pX7";

        [Ui("个人手机号", Mode = UiMode.AnchorDialog)]
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
                    if (PASS == password)
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

        [Ui("绑定为商家", Mode = UiMode.AnchorDialog)]
        public async Task bind(ActionContext ac)
        {
            User prin = (User) ac.Principal;

            if (prin.id == null)
            {
                ac.GivePane(200, (x) => { x.CALLOUT("请先设置个人手机号", false); });
                return;
            }

            string shopid = null;
            string password = null;
            if (ac.GET)
            {
                ac.GivePane(200, (x) =>
                {
                    x.CALLOUT("已经绑定为商家", false);
                    x.FORM_();
                    x.TEXT(nameof(shopid), shopid, label: "商家编号", max: 6, min: 6, pattern: "[0-9]+", required: true);
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
                    var credential = (string) dc.Scalar("SELECT credential FROM shops WHERE id = @1 AND mgrid = @2", p => p.Set(shopid).Set(prin.id));
                    if (credential != null && credential.EqualsCredential(shopid, password))
                    {
                        dc.Execute("UPDATE users SET oprat = @1, opr = @2 WHERE wx = @3; UPDATE shops SET mgrwx = @3 WHERE id = @1", p => p.Set(shopid).Set(User.MANAGER).Set(prin.wx));
                        prin.oprat = shopid;

                        // update token
                        prin.oprat = shopid;
                        prin.opr = User.MANAGER;
                        ac.SetTokenCookie(prin, -1);

                        ac.GivePane(200);
                    }
                    else
                    {
                        ac.GivePane(200, (x) => { x.CALLOUT("输入的编号或密码有误；或者没有绑定为商家的权限", false); });
                    }
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