using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The directory service controller.
    /// </summary>
    public class DirServiceDo : AbstServiceDo
    {
        public DirServiceDo(WebConfig cfg) : base(cfg)
        {
            AddChild<UserModuleDo>("user");
        }

    }
    
}