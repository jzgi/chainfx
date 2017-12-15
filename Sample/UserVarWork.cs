using Greatbone.Core;

namespace Greatbone.Samp
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig wc) : base(wc)
        {
        }
    }

    public class OprUserVarWork : UserVarWork
    {
        public OprUserVarWork(WorkConfig wc) : base(wc)
        {
        }
    }

    public class AdmOprVarWork : UserVarWork
    {
        public AdmOprVarWork(WorkConfig wc) : base(wc)
        {
        }
    }
}