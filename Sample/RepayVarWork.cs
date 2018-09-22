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

    public class RegRepayVarWork : RepayVarWork
    {
        public RegRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}