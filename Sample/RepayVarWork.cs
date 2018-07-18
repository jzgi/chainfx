using Greatbone;

namespace Samp
{
    public abstract class RepayVarWork : Work
    {
        protected RepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class OrgRepayVarWork : RepayVarWork
    {
        public OrgRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class PlatRepayVarWork : RepayVarWork
    {
        public PlatRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}