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
    [Ui("人员")]
    public class CtrWork : UserWork<CtrUserVarWork>
    {
        public CtrWork(WorkConfig cfg) : base(cfg)
        {
            Create<CtrOrderWork>("mgro");

            Create<GvrOrderWork>("gvro");

            Create<DvrOrderWork>("dvro");

            Create<CtrItemWork>("item");

            Create<CtrOrgWork>("org");

            Create<CtrRepayWork>("repay");
        }

        public void @default(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT * FROM users WHERE ctr > 0 ORDER BY ctr");
                    var arr = dc.Query<User>();
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR();
                        h.TABLE(arr, null,
                            o => h.TD(o.name).TD(o.tel).TD(Ctrs[o.ctr])
                        );
                    });
                }
            }
            else
            {
                wc.GiveFrame(200, false, 60 * 15, "调度作业");
            }
        }

        [UserAccess(CTR_MGR)]
        [Ui("添加", "添加中心操作人员"), Tool(ButtonShow, size: 1)]
        public async Task add(WebContext wc, int cmd)
        {
            string tel = null;
            short ctr = 0;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDSET_("添加人员");
                    h.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11);
                    h.SELECT(nameof(ctr), ctr, Ctrs, "角色");
                    h._FIELDSET();
                    h._FORM();
                });
            }
            else
            {
                if (cmd == 2) // add
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Execute("UPDATE users SET ctr = @1 WHERE tel = @2", p => p.Set(ctr).Set(tel));
                    }
                }
                wc.GivePane(200);
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