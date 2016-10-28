using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The common base class for all module controllers.
    /// </summary>
    ///
    public abstract class AbstModule : WebModule
    {
        public AbstModule(WebArg arg) : base(arg)
        {
        }

        public virtual void mgmt(WebContext wc, string subscpt)
        {
        }

    }

}