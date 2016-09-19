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
            AddSub<FameHub>("fame", false);

            AddSub<BrandHub>("brand", false);
        }
    }
}