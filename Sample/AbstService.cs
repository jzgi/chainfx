using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : WebService
    {
        internal readonly Admin[] admins;

        public AbstService(WebServiceContext sc) : base(sc)
        {
            admins = JsonUtility.FileToArray<Admin>(sc.GetFilePath("$admins.json"));
        }
    }
}