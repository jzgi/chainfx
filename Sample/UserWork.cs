using Greatbone.Core;
using static Greatbone.Core.Proj;

namespace Greatbone.Sample
{
    public class UserWork<V> : Work where V : UserVarWork
    {
        public UserWork(WorkContext wc) : base(wc)
        {
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                dc.Sql("SELECT ").columnlst(User.Empty, proj)._("FROM users ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query("SELECT * FROM users"))
                {
                    ac.GiveWorkPage(Parent, 200, dc.ToList<User>()); // ok
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }

        [Ui("按城市", Mode = UiMode.Button)]
        public void srch(ActionContext ac)
        {
        }

        [Ui("查找编号", Mode = UiMode.AnchorDialog)]
        public void find(ActionContext ac)
        {
            if (ac.GET)
            {
                string id = null;
                ac.GiveDialogForm(200, f =>
                {
                    f.TEXT(nameof(id), id, label: "用户编号", max: 11, min: 11);
                });
            }
            else
            {
                string id = ac.Query[nameof(id)];

                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ BIN ^ TRANSF ^ SECRET;
                    dc.Sql("SELECT ").columnlst(User.Empty, proj)._("FROM users WHERE id = @1");
                    if (dc.Query(p => p.Set(id)))
                    {
                        ac.GiveWorkPage(Parent, 200, dc.ToList<User>()); // ok
                    }
                    else
                    {
                        ac.Give(204); // no content
                    }
                }
            }
        }

        [Ui]
        public void aggr(ActionContext ac)
        {
        }
    }

    public class MyUserWork : UserWork<MyUserVarWork>
    {
        public MyUserWork(WorkContext wc) : base(wc)
        {
            CreateVar<MyUserVarWork>((prin) => ((User)prin).wx);
        }
    }

    public class AdmUserWork : UserWork<MyUserVarWork>
    {
        public AdmUserWork(WorkContext wc) : base(wc)
        {
        }
    }
}