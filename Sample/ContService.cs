using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The content service controller.
    /// </summary>
    public class ContService : AbsService
    {
        public ContService(WebConfig cfg) : base(cfg)
        {
            AddControl<PostModule>("post", false);
            
            AddControl<NoticeModule>("notice", false);
        }

    }
    
}