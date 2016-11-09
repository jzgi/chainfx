using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The content service controller.
    /// </summary>
    public class ContServiceWork : AbstServiceWork
    {
        public ContServiceWork(WebConfig cfg) : base(cfg)
        {
            AddChild<PostWork>("post");
            
            AddChild<NoticeWork>("notice");
        }

    }
    
}