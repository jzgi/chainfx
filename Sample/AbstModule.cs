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
        public AbstModule(WebHierarchyContext whc) : base(whc)
        {
        }

        public virtual void mgmt(WebContext wc, string subscpt)
        {
        }

    }

}