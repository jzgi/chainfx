using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFrame(200);
        }

        [Ui("基本资料", UiMode.AnchorDialog)]
        public void profile(ActionContext ac)
        {
            string userid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query1("SELECT * FROM users WHERE id = @1", p => p.Set(userid)))
                {
                    ac.GiveFormPane(200, dc.ToObject<User>());
                }
                else
                {
                    ac.Give(404); // not found
                }
            }
        }

        [Ui("设置密码", UiMode.AnchorDialog)]
        public void pwd(ActionContext ac)
        {
        }
    }

    public class MyUserVarWork : UserVarWork
    {
        public MyUserVarWork(WorkContext wc) : base(wc)
        {
            Create<MyCartOrderWork>("cart");

            Create<MyRealOrderWork>("real");
        }
    }

    public class DvrUserVarWork : UserVarWork
    {
        public DvrUserVarWork(WorkContext wc) : base(wc)
        {
            Create<DvrReadyOrderWork>("ready"); // orders ready to ship

            Create<DvrShippedOrderWork>("shipped"); // orders shipped

            Create<DvrRepayWork>("repay"); // deliverer repay management
        }
    }

    public class AdmUserVarWork : UserVarWork
    {
        public AdmUserVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}