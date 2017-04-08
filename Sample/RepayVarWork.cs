using Greatbone.Core;

namespace Greatbone.Sample
{
    public class RepayVarWork : Work
    {
        public RepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class OprRepayVarWork : RepayVarWork
    {
        public OprRepayVarWork(WorkContext fc) : base(fc)
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