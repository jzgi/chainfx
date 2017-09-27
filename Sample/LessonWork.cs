using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class LessonWork<V> : Work where V : ChatVarWork
    {
        protected LessonWork(WorkContext wc) : base(wc)
        {
        }
    }

    [Ui("客服")]
    [User(User.AID)]
    public class AdmChatWork : ChatWork<OprChatVarWork>
    {
        public AdmChatWork(WorkContext wc) : base(wc)
        {
            CreateVar<OprChatVarWork, string>((prin) => ((Chat) prin).wx);
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM chats WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Chat>(), (h, o) => { });
                }
                else
                {
                    ac.GiveGridPage(200, (Chat[]) null, null);
                }
            }
        }
    }
}