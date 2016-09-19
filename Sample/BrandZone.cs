using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandZone : WebSection
    {
        public BrandZone(WebConfig cfg) : base(cfg)
        {
            SetVarHub<BrandVarHub>(false);
        }
    }
}