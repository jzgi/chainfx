using System.Threading.Tasks;
using Greatbone.Core;
using static Greatbone.Core.Modal;

namespace Greatbone.Sample
{
    [User]
    public abstract class SlideVarWork : Work
    {
        protected SlideVarWork(WorkConfig cfg) : base(cfg)
        {
        }
    }

    public class AdmSlideVarWork : SlideVarWork
    {
        public AdmSlideVarWork(WorkConfig cfg) : base(cfg)
        {
        }

        [Ui("回复"), Tool(ButtonShow)]
        public async Task reply(WebContext ac)
        {
        }
    }
}