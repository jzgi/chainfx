using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandHub : WebHub
    {
        public BrandHub(WebConfig cfg) : base(cfg)
        {
            SetVarSub<BrandVarSub>(false);
        }
    }
}