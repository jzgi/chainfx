using Greatbone.Core;

namespace Greatbone.Sample
{
    public abstract class LessonWork<V> : Work where V : LessonVarWork
    {
        protected LessonWork(WorkContext wc) : base(wc)
        {
        }
    }

    [Ui("客服")]
    [User(User.OPR_)]
    public class AdmLessonWork : LessonWork<AdmLessonVarWork>
    {
        public AdmLessonWork(WorkContext wc) : base(wc)
        {
            CreateVar<AdmLessonVarWork, string>((obj) => ((Lesson) obj).id);
        }

        public void @default(ActionContext ac)
        {
            string shopid = ac[typeof(ShopVarWork)];
            using (var dc = ac.NewDbContext())
            {
                if (dc.Query("SELECT * FROM chats WHERE shopid = @1", p => p.Set(shopid)))
                {
                    ac.GiveGridPage(200, dc.ToArray<Lesson>(), (h, o) => { });
                }
                else
                {
                    ac.GiveGridPage(200, (Lesson[]) null, null);
                }
            }
        }
    }
}