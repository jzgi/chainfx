using Greatbone.Core;
using static Greatbone.Core.Projection;

namespace Greatbone.Sample
{
    [Ui("用户管理")]
    public class UserFolder : Folder
    {
        public UserFolder(FolderContext fc) : base(fc)
        {
            CreateVar<UserVarFolder>((tok) => ((User)tok).id);
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                const int proj = -1 ^ BIN ^ CODE ^ HUMAN;
                dc.Sql("SELECT ").columnlst(Shop.Empty, proj)._("FROM users ORDER BY id LIMIT 30 OFFSET @1");
                if (dc.Query("SELECT * FROM users"))
                {
                    ac.GiveFolderPage(Parent, 200, dc.ToList<User>()); // ok
                }
                else
                {
                    ac.Give(204); // no content
                }
            }
        }

        [Ui("按城市", Dialog = 2, Get = true)]
        public void srch(ActionContext ac)
        {
        }

        [Ui("查找编号", Dialog = 2, Get = true)]
        public void find(ActionContext ac)
        {
            if (ac.GET)
            {
                string id = null;
                ac.GiveModalForm(200, f =>
                {
                    f.TEXT(nameof(id), id, Label: "用户编号", Max: 11, Min: 11);
                });
            }
            else
            {
                string id = ac.Query[nameof(id)];

                using (var dc = ac.NewDbContext())
                {
                    const int proj = -1 ^ BIN ^ CODE ^ HUMAN;
                    dc.Sql("SELECT ").columnlst(User.Empty, proj)._("FROM users WHERE id = @1");
                    if (dc.Query(p => p.Set(id)))
                    {
                        ac.GiveFolderPage(Parent, 200, dc.ToList<User>()); // ok
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