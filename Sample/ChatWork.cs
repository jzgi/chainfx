using Greatbone;
using static Core.User;

namespace Core
{
    public abstract class ChatWork<V> : Work where V : ChatVarWork
    {
        protected ChatWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [Ui("客服")]
    [User(OPR)]
    public class OprChatWork : ChatWork<OprChatVarWork>
    {
        public OprChatWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<OprChatVarWork, string>((prin) => ((User) prin).wx);
        }

        public void @default(WebContext wc)
        {
            string orgid = wc[typeof(OrgVarWork)];
            using (var dc = NewDbContext())
            {
//                if (dc.Query("SELECT * FROM chats WHERE shopid = @1", p => p.Set(shopid)))
//                {
//                    wc.GiveGridPage(200, dc.ToArray<Chat>(), (h, o) => { });
//                }
//                else
//                {
//                    wc.GiveGridPage(200, (Chat[]) null, null);
//                }
            }
        }
    }
}