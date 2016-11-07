using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service controller.
    /// </summary>
    public class BizServiceDo : AbstServiceDo
    {
        public BizServiceDo(WebConfig cfg) : base(cfg)
        {
            AddChild<FameModuleDo>("fame");

            AddChild<BrandModuleDo>("brand");
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }
}