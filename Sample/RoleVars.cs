using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.Org;
using static Samp.User;

namespace Samp
{
    [UserAccess]
    public class MyVarWork : Work
    {
        public MyVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<MyOrderWork>("ord");

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
                h.P("让您的好友扫分享码，成为TA的引荐人，一同享用健康产品。以后凡是TA下单购物，您也能得到相应的积分奖励。");
                h.QRCODE(SampUtility.NETADDR + "/my//join?refid=" + prin.id);
                h._COL();
                h._UL();
            }, title: "我的设置");
        }

        public void join(WebContext wc)
        {
            int myid = wc[this];
            int refid = wc.Query[nameof(refid)];
            using (var dc = NewDbContext())
            {
                dc.Execute("UPDATE users SET refid = @1 WHERE id = @2", p => p.Set(refid).Set(myid));
            }
            wc.GiveRedirect(SampUtility.JOINADDR);
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

    [UserAccess(ctr: 1)]
    [Ui("常规")]
    public class CtrVarWork : Work, IOrgVar
    {
        public CtrVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<CtrChatWork>("chat");

            Create<CtrOrderWork>("newo");

            Create<CtrItemWork>("item");

            Create<OrgCashWork>("cash");
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
                    h.LI("简　介", org.addr);
                    h.LI("经　理", org.mgrname, org.mgrtel);
                    h._UL();
                });
            }
        }

        [UserAccess(OPRMGR)]
        [Ui("人员"), Tool(ButtonOpen)]
        public async Task acl(WebContext wc, int cmd)
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
                h.BUTTON(nameof(acl), 1, "删除");

                h.FIELDSET_("添加人员");
                h.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11);
                h.SELECT(nameof(opr), opr, Oprs, "角色");
                h._FIELDSET();
                h.BUTTON(nameof(acl), 2, "添加");
                h._FORM();
            });
        }

        [UserAccess(OPRMEM)]
        [Ui("状态"), Tool(ButtonShow)]
        public async Task status(WebContext wc)
        {
            string orgid = wc[this];
            var org = Obtain<Map<string, Org>>()[orgid];
            User prin = (User) wc.Principal;
            bool custsvc;
            if (wc.GET)
            {
                custsvc = org.mgrwx == prin.wx;
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("设置网点营业状态").SELECT(nameof(org.status), org.status, Statuses, "营业状态")._FIELDSET();
                    h.FIELDSET_("客服设置");
                    h.CHECKBOX(nameof(custsvc), custsvc, "我要作为上线客服");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                org.status = f[nameof(org.status)];
                custsvc = f[nameof(custsvc)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE orgs SET status = @1 WHERE id = @2", p => p.Set(org.status).Set(orgid));
                    if (custsvc)
                    {
                        dc.Execute("UPDATE orgs SET oprwx = @1, oprtel = @2, oprname = @3 WHERE id = @4", p => p.Set(prin.wx).Set(prin.tel).Set(prin.name).Set(orgid));
                    }
                    else
                    {
                        dc.Execute("UPDATE orgs SET oprwx = NULL, oprtel = NULL, oprname = NULL WHERE id = @1", p => p.Set(orgid));
                    }
                }
                wc.GivePane(200);
            }
        }

        [Ui("零售点", "我的零售点"), Tool(AOpen)]
        public void pos(WebContext wc)
        {
            User prin = (User) wc.Principal;
            wc.GivePane(200, h =>
            {
                h.FORM_().FIELDSET_();
                h.QRCODE(SampUtility.NETADDR + "/list?posid=" + prin.id);
                h._FIELDSET()._FORM();
            }, false, 300);
        }
    }

    public class VdrVarWork : Work, IOrgVar
    {
        public VdrVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<CtrChatWork>("chat");
        }

        public void @default(WebContext wc)
        {
        }
    }


    [UserAccess(tm: 1)]
    [Ui("常规")]
    public class TmVarWork : Work, IOrgVar
    {
        public TmVarWork(WorkConfig cfg) : base(cfg)
        {
            Create<TmOrderWork>("ord");

            Create<OrgCashWork>("cash");
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