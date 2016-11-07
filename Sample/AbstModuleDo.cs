using Greatbone.Core;

namespace Greatbone.Sample
{

    ///
    /// <summary>
    /// The common base class for all module controllers.
    /// </summary>
    ///
    public abstract class AbstModuleDo : WebModuleDo
    {
        public AbstModuleDo(WebArg arg) : base(arg)
        {
        }

        public virtual void mgmt(WebContext wc, string subscpt)
        {
        }

    }

}