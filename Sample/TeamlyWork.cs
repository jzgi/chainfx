using Greatbone;

namespace Samp
{
    public class TeamlyWork : Work
    {
        public TeamlyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<TeamlyVarWork>(prin => ((User) prin).teamid);
        }
    }
}