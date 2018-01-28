using Greatbone.Core;

namespace Greatbone.Samp
{
    public abstract class SlideWork<V> : Work where V : SlideVarWork
    {
        protected SlideWork(WorkConfig cfg) : base(cfg)
        {
            CreateVar<V, string>((obj) => ((Slide) obj).title);
        }
    }

    [Ui("课程"), User(adm: true)]
    public class AdmSlideWork : SlideWork<AdmSlideVarWork>
    {
        public AdmSlideWork(WorkConfig cfg) : base(cfg)
        {
        }

        public void @default(ActionContext ac)
        {
            using (var dc = ac.NewDbContext())
            {
                dc.Query(dc.Sql("SELECT  ").columnlst(Slide.Empty).T(" FROM slides"));
                ac.GiveBoardPage(200, dc.ToArray<Slide>(), (h, o) =>
                {
                    h.CAPTION(o.title);
                    h.TAIL();
                });
            }
        }
    }
}