using System.Threading.Tasks;

namespace Greatbone.Core
{
    /// <summary>
    /// To filter before executing an action.
    /// </summary>
    public interface IBefore
    {
        bool Do(WebContext ac);
    }

    /// <summary>
    /// To filter asynchronously before executing an action.
    /// </summary>
    public interface IBeforeAsync
    {
        Task<bool> DoAsync(WebContext ac);
    }

    /// <summary>
    /// To filter after executing an action.
    /// </summary>
    public interface IAfter
    {
        void Do(WebContext ac);
    }

    /// <summary>
    /// To filter asynchronously after executing an action.
    /// </summary>
    public interface IAfterAsync
    {
        Task DoAsync(WebContext ac);
    }
}