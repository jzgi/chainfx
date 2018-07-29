using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Org;
using static Samp.User;

namespace Samp
{
    [UserAccess(stored: true)]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<MyOrdWork>("ord");

            Create<SampChatWork>("chat");
        }

        public void @default(WebContext wc)
        {
            var prin = (User) wc.Principal;
            wc.GivePage(200, h =>
            {
                h.UL_("uk-card- uk-card-primary uk-card-body");
                h.LI("用户昵称", prin.name);
                h.LI("电话", prin.tel);
                h.LI("地址", prin.addr);
                h.LI("剩余积分", prin.score);
                h._UL();
                h.UL_("uk-card- uk-card-primary uk-card-body");
                h.COL_();
                h.FI(null, "让您的好友扫分享码，成为TA的引荐人，一同享用健康产品。以后凡是TA下单购物，您也能得到相应的积分奖励。");
                h.QRCODE(SampUtility.NETADDR + "/my//join?refid=" + prin.id);
                h._COL();
                h._UL();
            }, title: "我的设置");
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
                        string credential = TextUtility.MD5(prin.tel + ":" + password);
                        dc.Execute("UPDATE users SET name = @1, tel = @2, credential = @3 WHERE id = @4", p => p.Set(name).Set(tel).Set(credential).Set(prin.id));
                    }
                    else // password no change
                    {
                        dc.Execute("UPDATE users SET name = @1, tel = @2 WHERE id = @3", p => p.Set(name).Set(tel).Set(prin.id));
                    }
                    prin.name = name;
                    prin.tel = tel;
                    wc.SetTokenCookie(prin, 0xff ^ PRIVACY);
                    wc.GivePane(200); // close dialog
                }
            }
        }
    }


    public class SupVarWork : Work, IOrgVar
    {
        public SupVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<CtrChatWork>("chat");
        }

        public void @default(WebContext wc)
        {
        }
    }


    [UserAccess(grp: 1)]
    [Ui("常规")]
    public class GrpVarWork : Work, IOrgVar
    {
        public GrpVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<TmOrdWork>("ord");

            Create<OrgRecWork>("cash");
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[this];
            var org = Obtain<Map<string, Org>>()[orgid];
            bool inner = wc.Query[nameof(inner)];
            if (!inner)
            {
                wc.GiveFrame(200, false, 60 * 15, org?.name);
            }
            else
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.UL_("uk-card uk-card-default uk-card-body");
                    h.LI("名称", org.name);
                    h.LI("地址", org.addr);
                    h.LI("地图", org.addr);
                    h.LI("负责人", org.mgrname, org.mgrtel);
                    h._UL();
                });
            }
        }
    }
}