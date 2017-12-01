using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class CashVarWork : Work
    {
        protected CashVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class OprCashVarWork : CashVarWork
    {
        public OprCashVarWork(WorkContext fc) : base(fc)
        {
        }
    }
}