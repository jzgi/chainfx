using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service
    /// </summary>
    public class DirService : WebService
    {
        public DirService(WebConfig cfg) : base(cfg)
        {
            AddSub<UserHub>("user", false);
        }

    }
}