using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class RepayVarWork : Work
    {
        protected RepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class OprRepayVarWork : RepayVarWork
    {
        public OprRepayVarWork(WorkContext fc) : base(fc)
        {
        }
    }

    public class DvrRepayVarWork : RepayVarWork
    {
        public DvrRepayVarWork(WorkContext fc) : base(fc)
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