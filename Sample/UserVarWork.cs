using Greatbone;

namespace Samp
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class CtrUserVarWork : UserVarWork
    {
        public CtrUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class GrpUserVarWork : UserVarWork
    {
        public GrpUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}