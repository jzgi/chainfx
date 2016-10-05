using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /brand/
    ///
    public class BrandHub : WebHub
    {
        public BrandHub(WebBuild bld) : base(bld)
        {
            SetVarHub<BrandVarHub>(false);
        }
    }
}