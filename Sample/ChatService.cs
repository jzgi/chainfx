using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ChatService : AbstService
    {
        public ChatService(ServiceContext sc) : base(sc)
        {
            // add sub folder
            CreateVar<ChatVarFolder>();
        }

        public void ACCESS_TOKEN(EventContext ec)
        {

        }
    }
}