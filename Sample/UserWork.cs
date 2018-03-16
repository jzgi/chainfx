using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Core.User;

namespace Core
{
    public abstract class UserWork<V> : Work where V : UserVarWork
    {
        protected UserWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>((obj) => ((User) obj).wx);
        }
    }

    [User(opr: OPRMGR)]
    [Ui("人员管理")]
    public class OprOprWork : UserWork<OprOprVarWork>
    {
        public OprOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            string orgid = wc[-1];
            using (var dc = NewDbContext())
            {
                var arr = dc.Query<User>("SELECT * FROM users WHERE oprat = @1", p => p.Set(orgid));
                wc.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.TABLEVIEW(arr,
                        h => h.TH("姓名").TH("电话").TH("网点").TH("岗位"),
                        (h, o) => h.TD(o.name).TD(o.tel).TD(o.city, o.oprat).TD(Oprs[o.opr])
                    );
                });
            }
        }

        [Ui("添加"), Tool(ButtonShow, 2)]
        public async Task add(WebContext wc, int cmd)
        {
            string orgid = wc[-1];
            string wx;
            string tel = null;
            short opr = 0;
            if (wc.GET)
            {
                wc.GivePane(200, m =>
                {
                    m.FORM_();
                    m.FIELDSET_("添加人员");
                    m.TEXT(nameof(tel), tel, label: "手机", pattern: "[0-9]+", max: 11, min: 11);
                    m.SELECT(nameof(opr), opr, Oprs, label: "角色");
                    m._FIELDSET();
                    m._FORM();
                });
            }
            else
            {
                var f = await wc.ReadAsync<Form>();
                tel = f[nameof(tel)];
                opr = f[nameof(opr)];
                using (var dc = NewDbContext())
                {
                    dc.Execute("UPDATE users SET opr = @1, oprat = @2 WHERE tel = @3", p => p.Set(opr).Set(orgid).Set(tel)); // may add multiple
                }
            }
        }


        [Ui("移除"), Tool(ButtonPickConfirm, 2)]
        public async Task rm(WebContext wc, int cmd)
        {
            var f = await wc.ReadAsync<Form>();
            string wx = f[nameof(wx)];
            using (var dc = NewDbContext())
            {
                dc.Execute("UPDATE users SET opr = 0, oprat = NULL WHERE wx = @1", p => p.Set(wx));
            }
        }
    }

    [Ui("人员")]
    public class AdmOprWork : UserWork<AdmOprVarWork>
    {
        public AdmOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext ac, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE opr > 0 ORDER BY city LIMIT 20 OFFSET @1");
                var arr = dc.Query<User>(p => p.Set(page * 20));
                ac.GivePage(200, m =>
                {
                    m.TOOLBAR();
                    m.TABLEVIEW(arr,
                        h => h.TH("姓名").TH("电话").TH("网点").TH("岗位"),
                        (h, o) => h.TD(o.name).TD(o.tel).TD(o.city, o.oprat).TD(Oprs[o.opr])
                    );
                });
            }
        }
    }
}