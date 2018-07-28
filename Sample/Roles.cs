using System.Threading.Tasks;
using Greatbone;
using static Samp.User;

namespace Samp
{
    [UserAccess(stored: false)]
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

    public class OprWork : Work
    {
        public OprWork(WorkConfig cfg) : base(cfg)
        {
            Create<OprNewOrdWork>("newo");

            Create<OprOldOrdWork>("oldo");

            Create<OprItemWork>("item");

            Create<OprRepayWork>("repay");

            Create<OprOrgWork>("org");

            Create<OprUserWork>("user");

            Create<OrgRecWork>("rec");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.T("<article class=\"uk-card uk-card-primary uk-card-body\">");
                    h.T("<h4>系统信息</h4>");
                    h.FI("版　本", "2.0");
                    h.T("</article>");
                });
            }
            else
            {
                wc.GiveFrame(200, false, 60 * 15, "平台管理");
            }
        }

        [UserAccess(OPRMGR)]
        [Ui("人员"), Tool(Modal.ButtonOpen)]
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
    }

    public class SupWork : Work
    {
        public SupWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<SupVarWork, string>(prin => ((User) prin).supat);
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