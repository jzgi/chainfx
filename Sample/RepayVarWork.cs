using Greatbone;
using static Greatbone.Modal;

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

    public class HubRepayVarWork : RepayVarWork
    {
        public HubRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}