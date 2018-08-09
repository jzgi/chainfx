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

    public class CtrUserVarWork : UserVarWork
    {
        public CtrUserVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [UserAccess(CTR_MGR)]
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