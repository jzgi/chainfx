using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandModule : WebModule
    {
        public BrandModule(WebSubConfig cfg) : base(cfg)
        {
            SetXHub<BrandXHub>(false);
        }
    }
}