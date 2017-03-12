using Greatbone.Core;

namespace Greatbone.Sample
{
    [Ui("用户管理")]
    public class UserFolder : Folder
    {
        public UserFolder(FolderContext fc) : base(fc)
        {
            CreateVar<UserVarFolder>();
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
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

        [Ui]
        public void srch(ActionContext ac)
        {
        }

        [Ui]
        public void aggr(ActionContext ac)
        {
        }
    }
}