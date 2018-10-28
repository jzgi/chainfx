using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class TeamWork : Work
    {
        protected TeamWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("团组")]
    public class HublyTeamWork : TeamWork
    {
        public HublyTeamWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<HublyTeamVarWork>();
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            wc.GivePage(200, h =>
            {
                h.TOOLBAR();
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Team.Empty).T(" FROM teams WHERE hubid = @1 ORDER BY name");
                    var arr = dc.Query<Team>(p => p.Set(hubid));
                    h.TABLE(arr, null,
                        o => h.TD_().H5(o.name).T("（").T(Team.Statuses[o.status]).T("）<br>").POI(o.x, o.y, o.name, o.addr)._TD().TD(o.mgrname)
                    );
                }
            });
        }

        [UserAuthorize(hubly: 7)]
        [Ui("新建"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.IsGet)
            {
                var o = new Team { };
                o.Read(wc.Query, 0);
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDUL_("填写网点资料");
                    h.LI_().TEXT("名　称", nameof(o.name), o.name, max: 10, required: true)._LI();
                    h.LI_().TEXT("地　址", nameof(o.addr), o.addr, max: 20)._LI();
                    h.LI_().NUMBER("经　度", nameof(o.x), o.x, min: 0.000000, max: 150.000000).NUMBER("纬　度", nameof(o.y), o.y, min: 0.000000, max: 150.00000)._LI();
                    h.LI_().SELECT("状　态", nameof(o.status), o.status, Team.Statuses, required: true)._LI();
                    h._FIELDUL()._FORM();
                });
            }
            else // POST
            {
                string hubid = wc[0];
                var o = await wc.ReadObjectAsync<Team>(0);
                o.hubid = hubid;
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO teams")._(Team.Empty, 0)._VALUES_(Team.Empty, 0);
                    dc.Execute(p => o.Write(p, 0));
                }
                wc.GivePane(200); // created
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("删除"), Tool(ButtonPickConfirm)]
        public async Task del(WebContext wc)
        {
            string hubid = wc[0];
            var f = await wc.ReadAsync<Form>();
            short[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("DELETE FROM teams WHERE hubid = @1 AND id")._IN_(key);
                dc.Execute(p => p.Set(hubid));
            }
            wc.GiveRedirect();
        }

        [UserAuthorize(hubly: 7)]
        [Ui("客户"), Tool(ButtonPrompt, size: 1)]
        public async Task search(WebContext wc, int cmd)
        {
            short hubly = 0;
            if (wc.IsGet)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_("设置操作岗位");
                    // h.LI_().SELECT("岗　位", nameof(hubly), hubly, Hubly)._LI();
                    h._FIELDUL();
                    h._FORM();
                });
            }
            else
            {
                string hubid = wc[0];
                var f = await wc.ReadAsync<Form>();
                hubly = f[nameof(hubly)];
                int[] key = f[nameof(key)];
                if (cmd == 2) // add
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("UPDATE users SET hubly = @1 WHERE hubid = @2 AND id ")._IN_(key);
                        dc.Execute(p => p.Set(hubly).Set(hubid));
                    }
                }
                wc.GivePane(200);
            }
        }
    }
}