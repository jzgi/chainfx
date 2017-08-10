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

    public class MyUserWork : UserWork<MyUserVarWork>
    {
        public MyUserWork(WorkContext wc) : base(wc)
        {
        }
    }

    [Ui("人员管理")]
    public class SprUserWork : UserWork<OprUserVarWork>
    {
        public SprUserWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            string city = ac[-1];
            using (var dc = ac.NewDbContext())
            {
                const ushort proj = 0xffff ^ User.CREDENTIAL;
                dc.Sql("SELECT ").columnlst(User.Empty, proj)._("FROM users WHERE city = @1 AND opr <> 0 ORDER BY id LIMIT 20 OFFSET @2");
                if (dc.Query(p => p.Set(city).Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<User>()); // ok
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }
    }


    [Ui("监管员")]
    public class AdmUserWork : UserWork<AdmUserVarWork>
    {
        public AdmUserWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac, int page)
        {
            using (var dc = ac.NewDbContext())
            {
                const ushort proj = 0xffff ^ User.CREDENTIAL;
                dc.Sql("SELECT ").columnlst(User.Empty, proj)._("FROM users WHERE sprat IS NOT NULL ORDER BY city LIMIT 20 OFFSET @1");
                if (dc.Query(p => p.Set(page * 20)))
                {
                    ac.GiveGridPage(200, dc.ToArray<User>()); // ok
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }
    }
}