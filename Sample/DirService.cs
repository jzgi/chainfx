using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The directory service controller.
    ///
    public class DirService : AbstService
    {
        public DirService(WebConfig cfg) : base(cfg)
        {
            Add<UserDir>("user");
        }
    }
}