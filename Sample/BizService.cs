using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The business service controller.
    ///
    public class BizService : AbstService
    {
        public BizService(WebConfig cfg) : base(cfg)
        {
            AddChild<FameDir>("fame");

            AddChild<BrandDir>("brand");
        }

        public void @default(WebContext wc, string subscpt)
        {
        }
    }
}