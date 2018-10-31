using Greatbone;

namespace Samp
{
    public class ShoplyWork : Work
    {
        public ShoplyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<ShoplyVarWork>(prin => ((User) prin).shopid);
        }
    }
}