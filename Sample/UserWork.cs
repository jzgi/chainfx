using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class UserWork : Work
    {
        protected UserWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("客户")]
    public class TeamlyUserWork : UserWork
    {
        public TeamlyUserWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<TeamUserVarWork>();
        }

        const int PageSize = 30;

        [Ui("全部"), Tool(Anchor)]
        public void @default(WebContext wc, int page)
        {
            string hubid = wc[0];
            short id = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE hubid = @1 AND teamid = @2 ORDER BY id DESC LIMIT ").T(PageSize).T(" OFFSET @3");
                var arr = dc.Query<User>(p => p.Set(hubid).Set(id).Set(page * PageSize));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null, o => h.TD_().T("<a href=\"/").T(hubid).T("/?forid=").T(o.id).T("\">").T(o.name).T("</a>")._TD().TD(o.tel).TD(o.addr));
                }, @public: false, maxage: 3, refresh: 720);
            }
        }

        [Ui(icon: "search"), Tool(AnchorPrompt, size: 1)]
        public void search(WebContext wc)
        {
            bool inner = wc.Query[nameof(inner)];
            string tel = null;
            if (inner)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDUL_("按手机号查找客户");
                    h.LI_().TEL("手　　机", nameof(tel), tel, pattern: "[0-9]+", max: 11, min: 3, required: true)._LI();
                    h._FIELDUL()._FORM();
                });
            }
            else // OUTER
            {
                string hubid = wc[0];
                tel = wc.Query[nameof(tel)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("SELECT * FROM users WHERE hubid = @1 AND tel LIKE @2");
                    var arr = dc.Query<User>(p => p.Set(hubid).Set(tel + "%"));
                    wc.GivePage(200, h =>
                    {
                        h.TOOLBAR();
                        h.TABLE(arr, null, o => h.TD(o.name).TD(o.tel).TD(o.addr));
                    }, @public: false, maxage: 3);
                }
            }
        }

        [Ui("新建", tip: "手工创建新客户"), Tool(ButtonShow)]
        public async Task @new(WebContext wc)
        {
            if (wc.GET)
            {
                var o = new User { };
                o.Read(wc.Query, 0);
                wc.GivePane(200, h =>
                {
                    h.FORM_().FIELDUL_("填写客户资料");
                    h.LI_().TEXT("客户名称", nameof(o.name), o.name, tip: "姓名或昵称", max: 4, min: 2, required: true)._LI();
                    h.LI_().TEXT("手　　机", nameof(o.tel), o.tel, pattern: "[0-9]+", tip: "个人手机号", max: 11, min: 11, required: true)._LI();
                    h.LI_().TEXT("收货地址", nameof(o.addr), o.addr, max: 30)._LI();
                    h._FIELDUL()._FORM();
                });
            }
            else // POST
            {
                string hubid = wc[0];
                short teamid = wc[Parent];
                var o = await wc.ReadObjectAsync(0, obj: new User
                {
                    hubid = hubid,
                    teamid = teamid
                });
                using (var dc = NewDbContext())
                {
                    dc.Sql("INSERT INTO users")._(User.Empty, 0)._VALUES_(User.Empty, 0);
                    dc.Execute(p => o.Write(p, 0));
                }
                wc.GivePane(201); // created
            }
        }
    }

    [Ui("操作")]
    public class TeamlyOprWork : UserWork
    {
        public TeamlyOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string hubid = wc[0];
            short id = wc[Parent];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE hubid = @1 AND teamid = @2 AND teamly > 0 ORDER BY id");
                var arr = dc.Query<User>(p => p.Set(hubid).Set(id));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD_().T(User.Teamly[o.teamly])
                    );
                }, @public: false, maxage: 3, refresh: 1800);
            }
        }

        [Ui("添加", tip: "添加操作人员"), Tool(ButtonShow)]
        public async Task add(WebContext wc)
        {
            string hubid = wc[0];
            short teamid = wc[Parent];
            short role = 0;
            string tel = null;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_("填写被添加人的手机号和操作权限");
                    h.LI_().TEXT("手　机", nameof(tel), tel, pattern: "[0-9]+", max: 11, min: 11, required: true)._LI();
                    h.LI_().SELECT("权　限", nameof(role), role, User.Teamly)._LI();
                    h._FIELDUL();
                    h._FORM();
                });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                tel = f[nameof(tel)];
                role = f[nameof(role)];
                using (var dc = NewDbContext())
                {
                    dc.Sql("UPDATE users SET teamid = @1, teamly = @2 WHERE hubid = @2 AND tel = @3");
                    dc.Execute(p => p.Set(teamid).Set(role).Set(tel));
                }
                wc.GivePane(200);
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("删除", "确定删除选中操作人员吗？"), Tool(ButtonPickConfirm)]
        public async Task del(WebContext wc)
        {
            string hubid = wc[0];
            var f = await wc.ReadAsync<Form>();
            short[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("DELETE FROM shops WHERE hubid = @1 AND id")._IN_(key);
                dc.Execute(p => p.Set(hubid));
            }
            wc.GiveRedirect();
        }
    }

    [Ui("人员")]
    public class ShoplyOprWork : UserWork
    {
        public ShoplyOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            short orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE shopat = @1 AND shoply > 0 ORDER BY name");
                var arr = dc.Query<User>(p => p.Set(orgid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR(group: 2);
                    h.TABLE(arr, null,
                        o => h.TD(o.name).TD(o.tel).TD(o.addr).TD_().T(User.Shoply[o.shoply])
                    );
                }, @public: false, maxage: 3, refresh: 300);
            }
        }

        [UserAuthorize(shoply: 7)]
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

    [Ui("操作")]
    public class HublyOprWork : UserWork
    {
        public HublyOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
            string hubid = wc[0];
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(User.Empty).T(" FROM users WHERE hubid = @1 AND hubly > 0 ORDER BY name");
                var arr = dc.Query<User>(p => p.Set(hubid));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr, null, o => h.TD(o.name).TD(o.tel).TD(User.Hubly[o.hubly]));
                });
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("添加", tip: "添加操作人员"), Tool(ButtonShow)]
        public async Task role(WebContext wc, int cmd)
        {
            string tel = null;
            short role = 0;
            if (wc.GET)
            {
                wc.GivePane(200, h =>
                {
                    h.FORM_();
                    h.FIELDUL_("填写被添加人的手机号和操作权限");
                    h.LI_().TEXT("手　机", nameof(tel), tel, pattern: "[0-9]+", max: 11, min: 11, required: true)._LI();
                    h.LI_().SELECT("权　限", nameof(role), role, User.Hubly)._LI();
                    h._FIELDUL();
                    h._FORM();
                });
            }
            else
            {
                string hubid = wc[0];
                var f = await wc.ReadAsync<Form>();
                tel = f[nameof(tel)];
                role = f[nameof(role)];
                if (cmd == 2) // add
                {
                    using (var dc = NewDbContext())
                    {
                        dc.Sql("UPDATE users SET hubly = @1 WHERE hubid = @2 AND tel = @3 ");
                        dc.Execute(p => p.Set(role).Set(hubid).Set(tel));
                    }
                }
                wc.GivePane(200);
            }
        }

        [UserAuthorize(hubly: 7)]
        [Ui("删除", "确定删除选中操作人员吗？"), Tool(ButtonPickConfirm)]
        public async Task del(WebContext wc)
        {
            string hubid = wc[0];
            var f = await wc.ReadAsync<Form>();
            short[] key = f[nameof(key)];
            using (var dc = NewDbContext())
            {
                dc.Sql("UPDATE users SET hubly = 0 WHERE hubid = @1 AND id")._IN_(key);
                dc.Execute(p => p.Set(hubid));
            }
            wc.GiveRedirect();
        }
    }
}