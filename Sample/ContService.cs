using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// The content service controller.
    ///
    public class ContService : AbstService
    {
        public ContService(WebConfig cfg) : base(cfg)
        {
            Add<PostDir>("post");

            Add<PostGrpDir>("postgrp");

            Add<NoticeDir>("notice");
        }
    }
}