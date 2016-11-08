using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service controller.
    /// </summary>
    public class DirService : AbstService
    {
        public DirService(WebConfig cfg) : base(cfg)
        {
            AddChild<UserModule>("user");
        }

    }
    
}