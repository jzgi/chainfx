using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : WebService
    {
        readonly Worker[] workers;

        public AbstService(WebConfig cfg) : base(cfg)
        {
            workers = JsonUtility.FileToArray<Worker>(cfg.GetFilePath("$workers.json"));
        }

        public void @default(WebActionContext ac)
        {

        }
    }
}