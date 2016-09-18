using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service.
    /// </summary>
    public class BizService : WebService
    {
        public BizService(WebServiceConfig cfg) : base(cfg)
        {
            AddSub<FameZone>("fame", false);

            AddSub<BrandZone>("brand", false);
        }
    }
}