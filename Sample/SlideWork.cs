using Greatbone.Core;

namespace Greatbone.Samp
{
    public abstract class SlideWork<V> : Work where V : SlideVarWork
    {
        protected SlideWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    [User(adm: true)]
    public class AdmSlideWork : SlideWork<AdmSlideVarWork>
    {
        public AdmSlideWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<AdmSlideVarWork, string>((obj) => ((Slide) obj).no);
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Query("SELECT * FROM slides");
                ac.GiveBoardPage(200, dc.ToArray<Slide>(), (h, o) => { });
            }
        }
    }
}