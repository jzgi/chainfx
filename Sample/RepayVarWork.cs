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

    public class CtrRepayVarWork : RepayVarWork
    {
        public CtrRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}