using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service controller.
    /// </summary>
    public class BizService : AbstService
    {
        public BizService(WebConfig cfg) : base(cfg)
        {
            AddChild<FameModule>("fame");

            AddChild<BrandModule>("brand");
        }

        public override void @default(WebContext wc, string subscpt)
        {
        }

    }
}