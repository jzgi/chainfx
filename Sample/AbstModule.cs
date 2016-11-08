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
        public AbstModule(WebNodeContext wnc) : base(wnc)
        {
        }

        public virtual void mgmt(WebContext wc, string subscpt)
        {
        }

    }

}