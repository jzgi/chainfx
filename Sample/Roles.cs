using System.Threading.Tasks;
using Greatbone;

namespace Samp
{
    [UserAccess(persisted: false)]
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
                prin.ctrid = wc.Query[nameof(prin.ctrid)];
                wc.GivePage(200, h =>
                {
                    h.FORM_();
                    h.HIDDEN(nameof(prin.refid), prin.refid);
                    h.HIDDEN(nameof(prin.ctrid), prin.ctrid);
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
                    const byte proj = 0xff ^ User.ID ^ User.LATER;
                    dc.Sql("INSERT INTO users ")._(prin, proj)._VALUES_(prin, proj);
                    dc.Execute(p => prin.Write(p));
                }
                wc.GiveRedirect(SampUtility.JOINADDR);
            }
        }
    }

    public class CtrWork : Work
    {
        public CtrWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<CtrVarWork, string>(prin => ((User) prin).ctrat);
        }
    }

    public class VdrWork : Work
    {
        public VdrWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<VdrVarWork, string>(prin => ((User) prin).vdrat);
        }
    }

    public class TmWork : Work
    {
        public TmWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<TmVarWork, string>(prin => ((User) prin).tmat);
        }
    }

    [UserAccess(plat: 1)]
    [Ui("常规")]
    public class PlatWork : Work
    {
        public PlatWork(WorkConfig cfg) : base(cfg)
        {
            Create<PlatUserWork>("user");

            Create<PlatCtrWork>("ctr");

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

        [Ui("清理"), Tool(Modal.ButtonOpen)]
        public void clean(WebContext ac)
        {
        }
    }
}