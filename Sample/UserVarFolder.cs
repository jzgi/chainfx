using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// This folder contains mainly a user's personal stuffs, including the shopping cart and orders.
    ///
    public class UserVarFolder : Folder, IVar
    {
        public UserVarFolder(FolderContext fc) : base(fc)
        {
            Create<CartFolder>("cart");

            Create<OrderFolder>("order");
        }
    }
}