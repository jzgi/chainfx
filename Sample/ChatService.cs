using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ChatService : AbstService
    {
        readonly Client WeiXinClient = new Client("https://api.weixin.qq.com");

        public ChatService(ServiceContext sc) : base(sc)
        {
            // add sub folder
            CreateVar<ChatVarFolder>();
        }
    }
}