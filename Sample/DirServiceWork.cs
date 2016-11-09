using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service controller.
    /// </summary>
    public class DirServiceWork : AbstServiceWork
    {
        public DirServiceWork(WebConfig cfg) : base(cfg)
        {
            AddChild<UserWork>("user");
        }

    }
    
}