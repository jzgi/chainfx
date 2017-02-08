using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /user/
    ///
    public class UserFolder : WebFolder
    {
        public UserFolder(WebFolderContext fc) : base(fc)
        {
            CreateVar<UserVarFolder>();
        }

        #region /user/

        /// search for a user by name.
        ///
        public void srch(WebActionContext ac)
        {
        }

        /// aggregate users information.
        ///
        public void aggr(WebActionContext ac)
        {
        }

        #endregion
    }
}