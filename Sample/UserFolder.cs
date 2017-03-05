using Greatbone.Core;

namespace Greatbone.Sample
{
    [User, Staff(-1, Ui = true)]
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

        [Staff(User.MARKETG)]
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