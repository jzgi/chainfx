using Greatbone.Core;

namespace Greatbone.Samp
{
    public abstract class LessonWork<V> : Work where V : LessonVarWork
    {
        protected LessonWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [User(adm: true)]
    public class AdmLessonWork : LessonWork<AdmLessonVarWork>
    {
        public AdmLessonWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<AdmLessonVarWork, string>((obj) => ((Lesson) obj).id);
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM slides");
                ac.GiveBoardPage(200, dc.ToArray<Lesson>(), (h, o) => { });
            }
        }
    }
}