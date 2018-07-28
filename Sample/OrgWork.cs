using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class OrgWork<V> : Work where V : OrgVarWork
    {
        protected OrgWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Org) obj).id);
        }
    }

    [Ui("机构")]
    public class OprOrgWork : OrgWork<CtrOrgVarWork>
    {
        public OprOrgWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs ORDER BY id");
                var arr = dc.Query<Org>();
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                        {
                            h.T("<section class=\"uk-card-header\">").T(o.id).SP().T(o.name).T("</section>");
                            h.UL_("uk-card-body");
                            h.LI("地　址", o.addr);
                            h.LI_("坐　标").T(o.x).SP().T(o.y)._LI();
                            h.LI_("经　理").T(o.mgrname).SP().T(o.tel)._LI();
                            h._UL();
                            h.VARTOOLS(css: "uk-card-footer");
                        }
                    );
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
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
    }

    [Ui("机构")]
    public class PlatCtrWork : OrgWork<PlatCtrVarWork>
    {
        public PlatCtrWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs ORDER BY id");
                var arr = dc.Query<Org>();
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.BOARD(arr, o =>
                        {
                            h.T("<section class=\"uk-card-header\">").T(o.id).SP().T(o.name).T("</section>");
                            h.UL_("uk-card-body");
                            h.LI("地　址", o.addr);
                            h.LI_("坐　标").T(o.x).SP().T(o.y)._LI();
                            h.LI_("经　理").T(o.mgrname).SP().T(o.tel)._LI();
                            h._UL();
                            h.VARTOOLS(css: "uk-card-footer");
                        }
                    );
                });
            }
        }

        [Ui("新建"), Tool(ButtonShow)]
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
    }
}