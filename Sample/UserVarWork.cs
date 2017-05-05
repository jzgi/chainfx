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
                    var user = dc.ToObject<User>();
                    ac.GivePane(200, null);
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

    [Ui("设置")]
    public class MyUserVarWork : UserVarWork
    {
        public MyUserVarWork(WorkContext wc) : base(wc)
        {
            Create<MyCartOrderWork>("cart");

            Create<MyCurrentOrderWork>("current");

            Create<MyHistoryOrderWork>("history");
        }
    }

    public class MgrUserVarWork : UserVarWork
    {
        public MgrUserVarWork(WorkContext wc) : base(wc)
        {
        }
    }

    public class AdmUserVarWork : UserVarWork
    {
        public AdmUserVarWork(WorkContext wc) : base(wc)
        {
        }
    }
}