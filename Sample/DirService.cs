using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service
    /// </summary>
    public class DirService : WebService
    {
        public DirService(WebServiceConfig cfg) : base(cfg)
        {
            AddSub<UserZone>("user", false);
        }

    }
}