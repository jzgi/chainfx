using Greatbone.Core;

namespace Greatbone.Sample
{
    public class RepayVarWork : Work
    {
        public RepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class MgrRepayVarWork : RepayVarWork
    {
        public MgrRepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class AdmRepayVarWork : RepayVarWork
    {
        public AdmRepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }
}