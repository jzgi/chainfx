using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandModule : WebModule
    {
        public BrandModule(WebConfig cfg) : base(cfg)
        {
            SetVarHub<BrandVarHub>(false);
        }
    }
}