using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service
    /// </summary>
    public class DirService : SampleService
    {
        public DirService(WebConfig cfg) : base(cfg)
        {
            AddSub<UserModule>("user", false);
        }

    }
    
}