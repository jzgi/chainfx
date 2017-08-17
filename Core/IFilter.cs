using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// <summary>
    /// To filter before executing an action.
    /// </summary>
    public interface IBefore
    {
        bool Do(ActionContext ac);
    }

    /// <summary>
    /// To filter asynchronously before executing an action.
    /// </summary>
    public interface IBeforeAsync
    {
        Task DoAsync(ActionContext ac);
    }

    /// <summary>
    /// To filter after executing an action.
    /// </summary>
    public interface IAfter
    {
        bool Do(ActionContext ac);
    }

    /// <summary>
    /// To filter asynchronously after executing an action.
    /// </summary>
    public interface IAfterAsync
    {
        Task DoAsync(ActionContext ac);
    }
}