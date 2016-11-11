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
            AddChild<PostDir>("post");

            AddChild<PostGrpDir>("postgrp");

            AddChild<NoticeDir>("notice");
        }
    }
}