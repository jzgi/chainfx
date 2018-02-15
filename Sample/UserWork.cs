using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class UserWork<V> : Work where V : UserVarWork
    {
        protected UserWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>((obj) => ((User) obj).wx);
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
                dc.Sql("SELECT ").columnlst(User.Empty).T(" FROM users WHERE opr > 0 ORDER BY city LIMIT 20 OFFSET @1");
                dc.Query(p => p.Set(page * 20));
                ac.GiveSheetPage(200, dc.ToArray<User>(),
                    h => h.TH("姓名").TH("电话").TH("网点").TH("岗位"),
                    (h, o) => h.TD(o.name).TD(o.tel).TD(o.city, o.oprat).TD(User.Oprs[o.opr])
                );
            }
        }
    }
}