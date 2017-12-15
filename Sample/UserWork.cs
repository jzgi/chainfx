using Greatbone.Core;

namespace Greatbone.Samp
{
    public abstract class UserWork<V> : Work where V : UserVarWork
    {
        protected UserWork(WorkConfig wc) : base(wc)
        {
            CreateVar<V, string>((obj) => ((User)obj).wx);
        }
    }

    [Ui("人员")]
    public class AdmOprWork : UserWork<AdmOprVarWork>
    {
        public AdmOprWork(WorkConfig wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(User.Empty).T(" FROM users WHERE opr <> 0 ORDER BY city LIMIT 20 OFFSET @1");
                dc.Query(p => p.Set(page * 20));
                ac.GiveSheetPage(200, dc.ToArray<User>(),
                    h => h.TH("姓名").TH("电话").TH("网点").TH("岗位"),
                    (h, o) => h.TD(o.name).TD(o.tel).TD(o.city, o.oprname).TD(User.Oprs[o.opr])
                );
            }
        }
    }
}