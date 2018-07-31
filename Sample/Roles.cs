using System.Threading.Tasks;
using Greatbone;
using static Samp.User;
using static Greatbone.Modal;

namespace Samp
{
    [UserAccess(true)]
    public class MyWork : Work
    {
        public MyWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<MyVarWork, int>((obj) => ((User) obj).id);
        }

        public async Task @ref(WebContext wc)
        {
            var prin = (User) wc.Principal;
            if (prin.id > 0)
            {
                wc.GivePage(200, h => { h.ALERT("您已经是会员，引荐操作无效。"); });
                return;
            }

            if (wc.GET)
            {
                prin.refid = wc.Query[nameof(prin.refid)];
                wc.GivePage(200, h =>
                {
                    h.FORM_();
                    h.HIDDEN(nameof(prin.refid), prin.refid);
                    h.FIELDSET_("会员信息");
                    h.TEXT(nameof(prin.name), prin.name, label: "姓名", max: 4, min: 2, required: true);
                    h.TEXT(nameof(prin.tel), prin.tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11, required: true);
                    h._FIELDSET();
                    h.ALERT("确认后请关注公众号");
                    h.BOTTOMBAR_().BUTTON("确定")._BOTTOMBAR();
                    h._FORM();
                });
            }
            else // POST
            {
                prin = await wc.ReadObjectAsync(obj: prin);
                using (var dc = NewDbContext())
                {
                    const byte proj = 0xff ^ ID ^ LATER;
                    dc.Sql("INSERT INTO users ")._(prin, proj)._VALUES_(prin, proj);
                    dc.Execute(p => prin.Write(p));
                }
                wc.GiveRedirect(SampUtility.JOINADDR);
            }
        }
    }

    [UserAccess(ctr: 1)]
    [Ui("中心")]
    public class CtrWork : OrgWork<CtrOrgVarWork>
    {
        public CtrWork(WorkConfig cfg) : base(cfg)
        {
            Create<CtrNewOrderWork>("newo");

            Create<CtrOldOrderWork>("oldo");

            Create<CtrItemWork>("item");

            Create<CtrRepayWork>("repay");

            Create<CtrUserWork>("user");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs ORDER BY id");
                        var arr = dc.Query<Org>();
                        h.TABLE(arr, null,
                            o => h.TD(o.name).TD(o.mgrname)
                        );

//                        h.BOARD(arr, o =>
//                        {
//                            h.T("<section class=\"uk-card-header\">").T(o.id).SP().T(o.name).T("</section>");
//                            h.UL_("uk-card-body");
//                            h.LI("地　址", o.addr);
//                            h.LI_("坐　标").T(o.x).SP().T(o.y)._LI();
//                            h.LI_("经　理").T(o.mgrname).SP().T(o.mgrtel)._LI();
//                            h._UL();
//                            h.VARTOOLS(css: "uk-card-footer");
//                        });
                    }
                });
            }
            else
            {
                wc.GiveFrame(200, false, 60 * 15, "后台作业功能");
            }
        }

        [Ui("新建团"), Tool(ButtonShow, Style.Primary)]
        public async Task @new(WebContext wc)
        {
            const byte proj = 0xff;
            if (wc.GET)
            {
                var o = new Org { };
                o.Read(wc.Query, proj);
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.TEXT(nameof(o.id), o.id, "编号", max: 4, min: 4, required: true);
                    m.TEXT(nameof(o.name), o.name, "名称", max: 10, required: true);
                    m.TEXT(nameof(o.addr), o.addr, "地址", max: 20);
                    m.NUMBER(nameof(o.x), o.x, "经度", max: 20).NUMBER(nameof(o.x), o.x, "纬度", max: 20);
                    m._FORM();
                });
            }

            else // post
            {
                var o = await wc.ReadObjectAsync<Org>(proj);
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO orgs")._(Org.Empty, proj)._VALUES_(Org.Empty, proj);
                    dc.Execute(p => o.Write(p, proj));
                }
                wc.GivePane(200); // created
            }
        }

        [UserAccess(CTR_MANAGER)]
        [Ui("人员", "设置操作人员"), Tool(ButtonOpen, size: 4)]
        public async Task acl(WebContext wc, int cmd)
        {
            string tel = null;
            short ctr = 0;
            var f = await wc.ReadAsync<Form>();
            if (f != null)
            {
                tel = f[nameof(tel)];
                ctr = f[nameof(ctr)];
                if (cmd == 1) // remove
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE users SET ctr = NULL WHERE tel = @1", p => p.Set(tel));
                    }
                }
                else if (cmd == 2) // add
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE users SET ctr = @1 WHERE tel = @2", p => p.Set(ctr).Set(tel));
                    }
                }
            }
            wc.GivePane(200, h =>
            {
                h.FORM_();
                h.FIELDSET_("现有人员");
                using (var dc = NewDbContext())
                {
                    if (dc.Query("SELECT name, tel, ctr FROM users WHERE ctr > 0"))
                    {
                        while (dc.Next())
                        {
                            dc.Let(out string name).Let(out tel).Let(out ctr);
                            h.RADIO(nameof(tel), tel, tel + " " + name + " " + Ctrs[ctr], false);
                        }
                    }
                }
                h._FIELDSET();
                h.BUTTON(nameof(acl), 1, "删除");

                h.FIELDSET_("添加人员");
                h.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11);
                h.SELECT(nameof(ctr), ctr, Ctrs, "角色");
                h._FIELDSET();
                h.BUTTON(nameof(acl), 2, "添加");
                h._FORM();
            });
        }

        [UserAccess(CTR_SUPPLIER)]
        [Ui("状态"), Tool(Modal.ButtonShow)]
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
                    //                    h.FIELDSET_("设置网点营业状态").SELECT(nameof(org.status), org.status, Statuses, "营业状态")._FIELDSET();
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

        [UserAccess(CTR)]
        [Ui("个款", "个人结款记录"), Tool(AOpen, size: 4)]
        public void repays(WebContext wc)
        {
            var prin = (User) wc.Principal;
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<Repay>("SELECT * FROM repays WHERE status = 2 AND uid = @1 ORDER BY id DESC", p => p.Set(prin.id));
                wc.GivePage(200, h =>
                {
                    h.TABLE(arr,
                        () => h.TH("人员").TH("期间").TH("订单").TH("金额").TH("转款"),
                        o => h.TD(Repay.Jobs[o.job], o.uname).TD_().T(o.fro).BR().T(o.till)._TD().TD(o.orders).TD(o.cash).TD(o.payer)
                    );
                });
            }
        }
    }

    public class GrpWork : Work
    {
        public GrpWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<GrpVarWork, string>(prin => ((User) prin).grpat);
        }
    }
}