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
            MakeVar<V>();
        }
    }

    [Ui("人员")]
    public class TeamlyUserWork : UserWork<TeamUserVarWork>
    {
        public TeamlyUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        const int PageSize = 30;

        public void @default(WebContext wc, int page)
        {
            string hubid = wc[0];
            short orgid = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE hubid = @1 AND teamat = @2 ORDER BY name LIMIT ").T(PageSize).T(" OFFSET @3");
                var arr = dc.Query<User>(p => p.Set(hubid).Set(orgid).Set(page * PageSize));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR(group: 2);
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD(o.addr).TD_().T(Teamly[o.teamly])
                    );
                }, @public: false, maxage: 6, refresh: 300);
            }
        }

        [UserAccess(hubly: 3)]
        [Ui("副手", tip: "副手设置"), Tool(ButtonPickConfirm)]
        public async Task aid(WebContext wc)
        {
            string hubid = wc[0];
            short orgid = wc[Parent];
            var f = await wc.ReadAsync<Form>();
            int[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE users SET teamly = CASE WHEN teamly = 1 THEN 0 ELSE 1 END WHERE hubid = @1 AND teamat = @2 AND teamly < 3 AND id")._IN_(key);
                dc.Execute(p => p.Set(hubid).Set(orgid), prepare: false);
            }
            wc.GiveRedirect();
        }
    }

    [Ui("人员")]
    public class ShopOprWork : UserWork<ShopUserVarWork>
    {
        public ShopOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            short orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE shopat = @1 AND shoply > 0 ORDER BY name");
                var arr = dc.Query<User>(p => p.Set(orgid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR(group: 2);
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD(o.addr).TD_().T(Shoply[o.shoply])
                    );
                }, @public: false, maxage: 3, refresh: 300);
            }
        }

        [UserAccess(shoply: 15)]
        [Ui("添加", icon: "plus"), Tool(ButtonPickConfirm)]
        public async Task add(WebContext wc, int cmd)
        {
            string shopid = wc[-1];
            var f = await wc.ReadAsync<Form>();
            int[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE users SET shop = CASE WHEN 1 THEN NULL ELSE THEN 1 END WHERE shopat = @1 AND shop < 15 AND id")._IN_(key);
                dc.Execute(p => p.Set(shopid));
            }
            wc.GiveRedirect();
        }
    }

    [Ui("人员")]
    public class HublyOprWork : UserWork<HublyOprVarWork>
    {
        public HublyOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE hubid = @1 AND hubly > 0 ORDER BY name");
                var arr = dc.Query<User>(p => p.Set(hubid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD(Hubly[o.hubly])
                    );
                });
            }
        }

        [UserAccess(hubly: 7)]
        [Ui("添加"), Tool(ButtonPickPrompt, size: 1)]
        public async Task role(WebContext wc, int cmd)
        {
            short hubly = 0;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_("设置操作岗位");
                    h.LI_().SELECT("岗　位", nameof(hubly), hubly, Hubly)._LI();
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

        [UserAccess(hubly: 7)]
        [Ui("删除"), Tool(ButtonPickPrompt, size: 1)]
        public async Task del(WebContext wc, int cmd)
        {
        }

    }
}