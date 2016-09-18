using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandZone : WebZone
    {
        public BrandZone(WebConfig cfg) : base(cfg)
        {
            SetVarHub<BrandVarHub>(false);
        }
    }
}