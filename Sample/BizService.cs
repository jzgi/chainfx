using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service.
    /// </summary>
    public class BizService : WebService
    {
        public BizService(WebConfig cfg) : base(cfg)
        {
            AddSub<FameModule>("fame", false);

            AddSub<BrandModule>("brand", false);
        }
    }
}