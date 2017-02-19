using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class AbstService : WebService<Token>
    {
        internal readonly Admin[] admins;

        public AbstService(WebServiceContext sc) : base(sc)
        {
            admins = JsonUtility.FileToArray<Admin>(sc.GetFilePath("$admins.json"));
        }
    }
}