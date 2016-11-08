using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service controller.
    /// </summary>
    public class BizServicer : AbstServicer
    {
        public BizServicer(WebConfig cfg) : base(cfg)
        {
            AddChild<FameController>("fame");

            AddChild<BrandController>("brand");
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }
}