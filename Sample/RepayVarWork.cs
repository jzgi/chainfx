using Greatbone;

namespace Samp
{
    public abstract class RepayVarWork : Work
    {
        protected RepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class TeamlyRepayVarWork : RepayVarWork
    {
        public TeamlyRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class ShoplyRepayVarWork : RepayVarWork
    {
        public ShoplyRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class HublyRepayVarWork : RepayVarWork
    {
        public HublyRepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}