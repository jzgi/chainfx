using Greatbone;

namespace Core
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class OprOprVarWork : UserVarWork
    {
        public OprOprVarWork(WorkConfig cfg) : base(cfg)
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