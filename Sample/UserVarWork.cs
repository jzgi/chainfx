using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class UserVarWork : Work
    {
        public UserVarWork(WorkContext fc) : base(fc)
        {
        }


        [Ui]
        public void cancel(ActionContext ac)
        {
        }

        [Ui("基本资料", Mode = UiMode.AnchorDialog)]
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

        [Ui("设置密码", Mode = UiMode.AnchorDialog)]
        public void pass(ActionContext ac)
        {
        }
    }

    [Ui("用户")]
    public class MyUserVarWork : UserVarWork
    {
        public MyUserVarWork(WorkContext fc) : base(fc)
        {
            Create<MyCartOrderWork>("cart");

            Create<MyRestOrderWork>("rest");
        }

        public void @default(ActionContext ac)
        {
            ac.GiveFramePage(200);
        }
    }

    public class AdmUserVarWork : UserVarWork
    {
        public AdmUserVarWork(WorkContext fc) : base(fc)
        {
        }
    }
}