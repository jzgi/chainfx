using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class OrgWork<V> : Work where V : OrgVarWork
    {
        protected OrgWork(WorkConfig cfg) : base(cfg)
        {
            MountVar<V>();
        }
    }

    [Ui("网点")]
    public class HubOrgWork : OrgWork<HubOrgVarWork>
    {
        public HubOrgWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("团"), Tool(Anchor, "uk-button-link")]
        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs WHERE hubid = @1 AND typ = 0 ORDER BY id");
                    var arr = dc.Query<Org>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.mgrname)
                    );
                }
            });
        }

        [Ui("产供"), Tool(Anchor, "uk-button-link")]
        public void shop(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Org.Empty).T(" FROM orgs WHERE hubid = @1 AND typ = 1 ORDER BY id");
                    var arr = dc.Query<Org>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.mgrname)
                    );
                }
            });
        }

        [UserAccess(hubly: 7)]
        [Ui("新建", "创建新网点"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            const byte proj = 0xff;
            if (wc.GET)
            {
                var o = new Org { };
                o.Read(wc.Query, proj);
                wc.GivePane(200, m =>
                {
                    m.FORM_().FIELDUL_("填写网点资料");
//                    m.LI_().TEXT("编　号", nameof(o.id), o.id, max: 4, min: 4, required: true)._LI();
//                    m.LI_().SELECT("类　型", nameof(o.hubid), o.hubid, Org.Typs, required: true)._LI();
                    m.LI_().TEXT("名　称", nameof(o.name), o.name, max: 10, required: true)._LI();
                    m.LI_().TEXT("地　址", nameof(o.addr), o.addr, max: 20)._LI();
                    m.LI_().NUMBER("经　度", nameof(o.x), o.x, max: 20).NUMBER("纬　度", nameof(o.x), o.x, max: 20)._LI();
                    m._FIELDUL()._FORM();
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