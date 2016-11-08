using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The content service controller.
    /// </summary>
    public class ContServicer : AbstServicer
    {
        public ContServicer(WebConfig cfg) : base(cfg)
        {
            AddChild<PostController>("post");
            
            AddChild<NoticeController>("notice");
        }

    }
    
}