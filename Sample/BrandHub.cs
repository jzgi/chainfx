using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandHub : WebHub
    {
        public BrandHub(WebBuild cfg) : base(cfg)
        {
            SetVarHub<BrandVarHub>(false);
        }
    }
}