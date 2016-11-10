using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The content service controller.
    /// </summary>
    public class ContService : AbstService
    {
        public ContService(WebConfig cfg) : base(cfg)
        {
            AddChild<PostDir>("post");
            
            AddChild<NoticeDir>("notice");
        }

    }
    
}