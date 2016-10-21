using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service controller.
    /// </summary>
    public class BizService : AbsService
    {
        public BizService(WebConfig cfg) : base(cfg)
        {
            AddControl<FameModule>("fame");

            AddControl<BrandModule>("brand");
        }

    }
}