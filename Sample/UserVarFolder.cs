using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// This folder contains mainly a user's personal stuffs, including the shopping cart and orders.
    ///
    [Ui("用户")]
    public class UserVarFolder : Folder, IVar
    {
        public UserVarFolder(FolderContext fc) : base(fc)
        {
            AddSub<OrderFolder>("order");
        }
    }
}