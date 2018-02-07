using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class RepayVarWork : Work
    {
        protected RepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class OprRepayVarWork : RepayVarWork
    {
        public OprRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class AdmRepayVarWork : RepayVarWork
    {
        public AdmRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}