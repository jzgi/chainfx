using Greatbone.Core;
using static Greatbone.Core.Proj;

namespace Greatbone.Sample
{
    [Ui("用户管理")]
    public class UserWork : Work
    {
        public UserWork(WorkContext wc) : base(wc)
        {
            CreateVar<UserVarWork>((prin) => ((User)prin).wx);
        }

        public void _(ActionContext ac)
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
                ac.GivePaneForm(200, f =>
                {
                    f.TEXT(nameof(id), id, Label: "用户编号", Max: 11, Min: 11);
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
}