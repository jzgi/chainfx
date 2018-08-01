using Greatbone;

namespace Samp
{
    public abstract class RepayVarWork : Work
    {
        protected RepayVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class MyRepayVarWork : RepayVarWork
    {
        public MyRepayVarWork(WorkConfig cfg) : base(cfg)
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