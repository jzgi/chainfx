using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The content service.
    /// </summary>
    public class ContService : WebService
    {
        public ContService(WebServiceConfig cfg) : base(cfg)
        {
            AddSub<PostModule>("post", false);
            AddSub<NoticeHub>("notice", false);
        }
    }
}