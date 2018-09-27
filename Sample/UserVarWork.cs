using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;
using static Samp.User;

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

        [UserAccess(RegMgmt)]
        [Ui("x", "删除此操作人员？"), Tool(ButtonConfirm)]
        public void rm(WebContext wc, int cmd)
        {
            int id = wc[this];
            using (var dc = NewDbContext())
            {
                dc.Execute("UPDATE users SET ctr = NULL WHERE id = @1", p => p.Set(id));
            }
        }
    }

    public class GrpUserVarWork : UserVarWork
    {
        public GrpUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }
}