using Greatbone.Core;

namespace Greatbone.Sample
{
    public class CommService : AbstService
    {
        readonly Client WeiXinClient = new Client("https://api.weixin.qq.com");

        public CommService(ServiceContext sc) : base(sc)
        {
            // add sub folder
            CreateVar<CommVarFolder>();
        }
    }
}