using System.Collections.Generic;
using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("用户")]
    public class UserVarWork : Work
    {
        public UserVarWork(WorkContext fc) : base(fc)
        {
        }

        public void _(ActionContext ac, int page)
        {
            string userid = ac[this];

            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM orders WHERE buywx = @1 ORDER BY id LIMIT 20 OFFSET @2", p => p.Set(userid).Set(page * 20)))
                {
                    ac.GiveWorkPage(null, 200, dc.ToList<Order>());
                }
                else
                {
                    ac.GiveWorkPage(null, 200, (List<Order>)null);
                }
            }
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
                    ac.GivePaneForm(200, dc.ToObject<User>());
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

    public class MyUserVarWork : UserVarWork
    {
        public MyUserVarWork(WorkContext fc) : base(fc)
        {
            Create<MyCartOrderWork>("cart");

            Create<MyRestOrderWork>("rest");
        }
    }

    public class AdmUserVarWork : UserVarWork
    {
        public AdmUserVarWork(WorkContext fc) : base(fc)
        {
        }
    }
}