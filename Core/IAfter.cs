using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// To filter before action.
    ///
    public interface IBefore
    {
        bool Do(ActionContext ac);
    }

    /// To filter before action.
    ///
    public interface IBeforeAsync
    {
        Task DoAsync(ActionContext ac);
    }
}
