using Greatbone.Core;

namespace Greatbone.Sample
{
    public class RepayVarWork : Work, IVar
    {
        public RepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class ShopRepayVarWork : RepayVarWork
    {
        public ShopRepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class SysRepayVarWork : RepayVarWork
    {
        public SysRepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }
}