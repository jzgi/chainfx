using Greatbone;

namespace Samp
{
    public class MyWork : Work
    {
        public MyWork(WorkConfig cfg) : base(cfg)
        {
            MakeVar<MyVarWork>((obj) => ((User) obj).id);
        }
    }
}