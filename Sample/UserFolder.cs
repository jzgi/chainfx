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
                    ac.ReplyFolderPage(200, dc.ToList<User>()); // ok
                }
                else
                {
                    ac.Reply(204); // no content
                }
            }
        }

        [User(false, User.MARKETG)]
        public void srch(ActionContext ac)
        {
        }

        /// aggregate users information.
        ///
        public void aggr(ActionContext ac)
        {
        }
    }
}