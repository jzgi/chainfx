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

    [Ui("人员管理")]
    public class AdmUserWork : UserWork<OprUserVarWork>
    {
        public AdmUserWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string city = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const int proj = 0xffff ^ User.CREDENTIAL;
                dc.Sql("SELECT ").columnlst(User.Empty, proj)._("FROM users WHERE opr <> 0 ORDER BY city LIMIT 20 OFFSET @2");
                if (dc.Query(p => p.Set(city).Set(page * 20)))
                {
                    ac.GiveSheetPage(200, dc.ToArray<User>()); // ok
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }
    }
}