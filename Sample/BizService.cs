using Greatbone.Core;

namespace Greatbone.Sample
{
    /// <summary>
    /// The business service.
    /// </summary>
    public class BizService : WebService
    {
        public BizService(WebConfig cfg) : base(cfg)
        {
            AddSub<FameModule>("fame", false);

            AddSub<BrandModule>("brand", false);
        }

        public override void @default(WebContext wc)
        {
            Roll<WebSub> subs = Subs;

            wc.SetHtml(200, html =>
            {

            });
            for (int i = 0; i < subs.Count; i++)
            {

            }
        }
    }
}