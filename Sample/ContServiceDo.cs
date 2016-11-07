using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The content service controller.
    /// </summary>
    public class ContServiceDo : AbstServiceDo
    {
        public ContServiceDo(WebConfig cfg) : base(cfg)
        {
            AddChild<PostModuleDo>("post");
            
            AddChild<NoticeModuleDo>("notice");
        }

    }
    
}