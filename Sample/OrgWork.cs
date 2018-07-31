using System.Threading.Tasks;
using Greatbone;
using static Greatbone.Modal;

namespace Samp
{
    public abstract class OrgWork<V> : Work where V : OrgVarWork
    {
        protected OrgWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>(obj => ((Org)obj).id);
        }
    }
}