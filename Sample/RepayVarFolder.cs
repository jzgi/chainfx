using Greatbone.Core;

namespace Greatbone.Sample
{
    public class RepayVarFolder : Folder, IVar
    {
        public RepayVarFolder(FolderContext fc) : base(fc)
        {
        }
    }

    public class ShopRepayVarFolder : RepayVarFolder
    {
        public ShopRepayVarFolder(FolderContext fc) : base(fc)
        {
        }
    }

    public class OpRepayVarFolder : RepayVarFolder
    {
        public OpRepayVarFolder(FolderContext fc) : base(fc)
        {
        }
    }
}