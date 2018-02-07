using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class AdmOprVarWork : UserVarWork
    {
        public AdmOprVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}