using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// To filter after action.
    ///
    public interface IAfter
    {
        bool Do(ActionContext ac);
    }

    /// To filter after action.
    ///
    public interface IAfterAsync
    {
        Task DoAsync(ActionContext ac);
    }
}
