using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service controller.
    /// </summary>
    public class DirServicer : AbstServicer
    {
        public DirServicer(WebConfig cfg) : base(cfg)
        {
            AddChild<UserController>("user");
        }

    }
    
}