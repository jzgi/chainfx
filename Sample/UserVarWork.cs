using Greatbone;

namespace Samp
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class PlatUserVarWork : UserVarWork
    {
        public PlatUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}