using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

namespace Samp
{
    public abstract class UserWork<V> : Work where V : UserVarWork
    {
        protected UserWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, int>((obj) => ((User) obj).id);
        }
    }

    [Ui("人员")]
    public class TeamUserWork : UserWork<TeamUserVarWork>
    {
        public TeamUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string teamid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE teamat = @1 ORDER BY name LIMIT 20 OFFSET @2");
                var arr = dc.Query<User>(p => p.Set(teamid).Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD_().T(Teamly[o.team])
                    );
                });
            }
        }

        [UserAccess(HUB_MGMT)]
        [Ui("加为帮手"), Tool(ButtonPickShow, size: 1)]
        public async Task add(WebContext wc, int cmd)
        {
            string tel = null;
            short ctr = 0;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_("添加人员");
                    h.LI_().TEXT("手　机", nameof(tel), tel, pattern: "[0-9]+", max: 11, min: 11)._LI();
                    h.LI_().SELECT("角　色", nameof(ctr), ctr, Hubly)._LI();
                    h._FIELDUL();
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

    [Ui("人员")]
    public class ShopUserWork : UserWork<ShopUserVarWork>
    {
        public ShopUserWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("人员")]
    public class HubUserWork : UserWork<HubUserVarWork>
    {
        public HubUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("客户"), Tool(A)]
        public void @default(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users ORDER BY id LIMIT 20 OFFSET @1");
                var arr = dc.Query<User>(p => p.Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr,
                        () => h.TH("姓名").TH("电话").TH("网点"),
                        o => h.TD(o.name).TD(o.tel).TD(o.teamat)
                    );
                });
            }
        }

        [Ui("查找"), Tool(APrompt)]
        public void find(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            string tel = null;
            if (inner)
            {
                wc.GivePane(200, h => { h.FORM_().FIELDUL_("手机号").TEL(null, nameof(tel), tel)._FIELDUL()._FORM(); });
            }
            else
            {
                tel = wc.Query[nameof(tel)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE tel = @1");
                    var arr = dc.Query<User>(p => p.Set(tel));
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR();
                        h.TABLE(arr,
                            () => h.TH("姓名").TH("电话").TH("网点"),
                            o => h.TD(o.name).TD(o.tel).TD(o.teamat)
                        );
                    });
                }
            }
        }

        [UserAccess(HUB_MGMT)]
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
                    h.FIELDUL_("添加人员");
                    h.LI_().TEXT("手　机", nameof(tel), tel, pattern: "[0-9]+", max: 11, min: 11)._LI();
                    h.LI_().SELECT("角　色", nameof(ctr), ctr, Hubly)._LI();
                    h._FIELDUL();
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
}