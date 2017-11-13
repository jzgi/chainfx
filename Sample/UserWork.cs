using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class UserWork<V> : Work where V : UserVarWork
    {
        protected UserWork(WorkContext wc) : base(wc)
        {
            CreateVar<V, string>((obj) => ((User) obj).wx);
        }
    }

    [Ui("人员")]
    public class AdmUserWork : UserWork<OprUserVarWork>
    {
        public AdmUserWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Sql("SELECT ").columnlst(User.Empty).T(" FROM users WHERE opr <> 0 ORDER BY city LIMIT 20 OFFSET @1");
                if (dc.Query(p => p.Set(page * 20)))
                {
                    ac.GiveTablePage(200, dc.ToArray<User>(),
                        h => h.TH("姓名").TH("电话").TH("城市").TH("地址").TH("操作网点").TH("操作岗位").TH("管理员"),
                        (h, o) => h.TD(o.name).TD(o.tel).TD(o.city).TD(o.addr).TD(o.oprat).TD(User.Oprs[o.opr]).TD(o.adm)
                    );
                }
                else
                {
                    ac.GiveTablePage(204, (User[]) null, null, null);
                }
            }
        }
    }
}