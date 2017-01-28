using Greatbone.Core;

namespace Greatbone.Sample
{
    ///
    /// /customer/-id-/
    ///
    public class CustomerVariableFolder : WebFolder, IVariable
    {
        public CustomerVariableFolder(WebFolderContext dc) : base(dc)
        {
        }

        ///
        ///
        public void cartadd(WebActionContext ac)
        {

        }

        ///
        ///
        public void cartrm(WebActionContext ac)
        {
            // string shopid = wc.Var(null);
        }

        ///
        ///
        public void checkout(WebActionContext ac)
        {
            // string shopid = wc.Var(null);

        }
    }
}