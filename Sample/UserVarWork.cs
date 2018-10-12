using Greatbone;

namespace Samp
{
    public abstract class UserVarWork : Work
    {
        protected UserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class TeamUserVarWork : UserVarWork
    {
        public TeamUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class ShopUserVarWork : UserVarWork
    {
        public ShopUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class HubUserVarWork : UserVarWork
    {
        public HubUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(WebContext wc)
        {
        }
    }
}