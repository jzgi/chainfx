using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /user/
    ///
    public class UserFolder : Folder
    {
        public UserFolder(FolderContext fc) : base(fc)
        {
            CreateVar<UserVarFolder>();
        }

        #region /user/

        /// search for a user by name.
        ///
        public void srch(ActionContext ac)
        {
        }

        /// aggregate users information.
        ///
        public void aggr(ActionContext ac)
        {
        }

        #endregion
    }
}