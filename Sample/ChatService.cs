using Greatbone.Core;

namespace Greatbone.Sample
{
    public class ChatService : AbstService
    {
        public ChatService(ServiceContext sc) : base(sc)
        {
            CreateVar<ChatVarWork>();
        }

        public void ACCESS_TOKEN(EventContext ec)
        {

        }
    }
}