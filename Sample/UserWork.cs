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

    [Ui("用户管理")]
    public class CtrUserWork : UserWork<PlatUserVarWork>
    {
        public CtrUserWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("员工"), Tool(A)]
        public void @default(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE oprat IS NOT NULL ORDER BY oprat LIMIT 20 OFFSET @1");
                var arr = dc.Query<User>(p => p.Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr,
                        () => h.TH("姓名").TH("电话").TH("网点").TH("岗位"),
                        o => h.TD(o.name).TD(o.tel).TD(o.supat).TD(Oprs[o.opr])
                    );
                });
            }
        }

        [Ui("客户"), Tool(A)]
        public void all(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users ORDER BY id LIMIT 20 OFFSET @1");
                var arr = dc.Query<User>(p => p.Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLE(arr,
                        () => h.TH("姓名").TH("电话").TH("网点").TH("积分"),
                        o => h.TD(o.name).TD(o.tel).TD(o.supat).TD(o.score)
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
                wc.GivePane(200, h => { h.FORM_().FIELDSET_("手机号").TEL(nameof(tel), tel)._FIELDSET()._FORM(); });
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
                            () => h.TH("姓名").TH("电话").TH("网点").TH("积分"),
                            o => h.TD(o.name).TD(o.tel).TD(o.supat).TD(o.score)
                        );
                    });
                }
            }
        }
    }
}