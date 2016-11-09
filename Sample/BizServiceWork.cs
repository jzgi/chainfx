using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service controller.
    /// </summary>
    public class BizServiceWork : AbstServiceWork
    {
        public BizServiceWork(WebConfig cfg) : base(cfg)
        {
            AddChild<FameWork>("fame");

            AddChild<BrandWork>("brand");
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }
}