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

    [Ui("成员")]
    public class TeamUserWork : UserWork<TeamUserVarWork>
    {
        public TeamUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        const int PageSize = 30;

        [Ui("正式"), Tool(Anchor, "uk-button-link")]
        public void @default(WebContext wc, int page)
        {
            short teamid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE teamat = @1 AND teamly > 0 ORDER BY name LIMIT ").T(PageSize).T(" OFFSET @2");
                var arr = dc.Query<User>(p => p.Set(teamid).Set(page * PageSize));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR(group: 1);
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD(o.addr).TD_().T(Teamly[o.teamly])
                    );
                }, @public: false, maxage: 3, refresh: 300);
            }
        }

        [Ui("待批"), Tool(Anchor, "uk-button-link")]
        public void pre(WebContext wc, int page)
        {
            short teamid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE teamat = @1 AND teamly = 0 ORDER BY name LIMIT ").T(PageSize).T(" OFFSET @2");
                var arr = dc.Query<User>(p => p.Set(teamid).Set(page * PageSize));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR(group: 2);
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD(o.addr).TD_().T(Teamly[o.teamly])
                    );
                }, @public: false, maxage: 3, refresh: 300);
            }
        }

        [UserAccess(hubly: 7)]
        [Ui("副手", group: 1), Tool(ButtonPickConfirm)]
        public async Task aid(WebContext wc, int cmd)
        {
            string teamid = wc[-1];
            var f = await wc.ReadAsync<Form>();
            int[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE users SET team = CASE WHEN 1 THEN NULL ELSE THEN 1 END WHERE teamat = @1 AND team <> 15 AND id")._IN_(key);
                dc.Execute(p =>
                {
                    p.Set(teamid);
                    p.SetIn(key);
                });
            }
            wc.GiveRedirect();
        }

        [Ui("批准", group: 2), Tool(ButtonPickConfirm)]
        public async Task apprv(WebContext wc, int cmd)
        {
            string teamid = wc[-1];
            var f = await wc.ReadAsync<Form>();
            int[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE users SET team = CASE WHEN 1 THEN NULL ELSE THEN 1 END WHERE teamat = @1 AND team <> 15 AND id")._IN_(key);
                dc.Execute(p =>
                {
                    p.Set(teamid);
                    p.SetIn(key);
                });
            }
            wc.GiveRedirect();
        }
    }

    [Ui("人员")]
    public class ShopUserWork : UserWork<ShopUserVarWork>
    {
        public ShopUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            wc.GivePage(200, h => { h.TOOLBAR(); });
        }

        [Ui("查找"), Tool(AnchorPrompt)]
        public void find(WebContext wc)
        {
            string shopid = wc[-1];
            string tel = null;
            bool inner = wc.Query[nameof(inner)];
            if (inner)
            {
                wc.GivePane(200, h => { h.FORM_().FIELDUL_("手机号").TEL(null, nameof(tel), tel)._FIELDUL()._FORM(); });
            }
            else
            {
                tel = wc.Query[nameof(tel)];
                using (var dc = NewDbContext())
                {
                    var arr = dc.Query<User>("SELECT * FROM users WHERE shopat = @1 AND tel = @2", p => p.Set(shopid).Set(tel));
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR(title: tel);
                        h.TABLE(arr, null,
                            o => h.TD(o.name).TD(o.tel).TD(o.addr).TD_().T(Shoply[o.shoply])
                        );
                    });
                }
            }
        }

        [UserAccess(shoply: 15)]
        [Ui("加减助手"), Tool(ButtonPickConfirm)]
        public async Task add(WebContext wc, int cmd)
        {
            string shopid = wc[-1];
            var f = await wc.ReadAsync<Form>();
            int[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE users SET shop = CASE WHEN 1 THEN NULL ELSE THEN 1 END WHERE shopat = @1 AND shop < 15 AND id")._IN_(key);
                dc.Execute(p =>
                {
                    p.Set(shopid);
                    p.SetIn(key);
                });
            }
            wc.GiveRedirect();
        }
    }

    [Ui("人员")]
    public class HubUserWork : UserWork<HubUserVarWork>
    {
        public HubUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("客户"), Tool(Anchor)]
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

        [Ui("查找"), Tool(AnchorPrompt)]
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

        [UserAccess(hubly: 7)]
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