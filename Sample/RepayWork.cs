using Greatbone.Core;

namespace Greatbone.Sample
{
    public class RepayWork : Work, IVar
    {
        public RepayWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class ShopRepayWork : RepayWork
    {
        public ShopRepayWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class SysRepayWork : RepayWork
    {
        public SysRepayWork(WorkContext fc) : base(fc)
        {
        }
    }
}