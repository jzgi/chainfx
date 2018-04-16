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
            CreateVar<V, string>((obj) => ((User)obj).wx);
        }
    }

    [Ui("人员")]
    public class AdmOprWork : UserWork<AdmOprVarWork>
    {
        public AdmOprWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc, int page)
        {
            using (var dc = NewDbContext())
            {
                dc.Sql("SELECT ").collst(Empty).T(" FROM users WHERE oprat IS NOT NULL ORDER BY oprat LIMIT 20 OFFSET @1");
                var arr = dc.Query<User>(p => p.Set(page * 20));
                wc.GivePage(200, h =>
                {
                    h.TOOLBAR();
                    h.TABLEVIEW(arr,
                        () => h.TH("姓名").TH("电话").TH("网点").TH("岗位"),
                        o => h.TD(o.name).TD(o.tel).TD(o.oprat).TD(Oprs[o.opr])
                    );
                });
            }
        }
    }
}