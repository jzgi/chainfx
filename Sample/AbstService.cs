using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : WebService
    {
        readonly Worker[] workers;

        public AbstService(WebServiceContext sc) : base(sc)
        {
            workers = JsonUtility.FileToArray<Worker>(sc.GetFilePath("$workers.json"));
        }
    }
}