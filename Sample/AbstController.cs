using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The common base class for all module controllers.
    /// </summary>
    ///
    public abstract class AbstController : WebController
    {
        public AbstController(WebArg arg) : base(arg)
        {
        }

        public virtual void mgmt(WebContext wc, string subscpt)
        {
        }

    }

}