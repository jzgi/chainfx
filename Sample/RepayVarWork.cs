using Greatbone.Core;

namespace Greatbone.Samp
{
    public abstract class RepayVarWork : Work
    {
        protected RepayVarWork(WorkConfig fc) : base(fc)
        {
        }
    }

    public class OprRepayVarWork : RepayVarWork
    {
        public OprRepayVarWork(WorkConfig fc) : base(fc)
        {
        }
    }

    public class AdmRepayVarWork : RepayVarWork
    {
        public AdmRepayVarWork(WorkConfig fc) : base(fc)
        {
        }
    }
}