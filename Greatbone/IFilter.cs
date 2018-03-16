using System.Threading.Tasks;

namespace Greatbone
{
    /// <summary>
    /// To filter before executing a procedure.
    /// </summary>
    public interface IBefore
    {
        bool Do(WebContext ac);
    }

    /// <summary>
    /// To filter asynchronously before executing a procedure.
    /// </summary>
    public interface IBeforeAsync
    {
        Task<bool> DoAsync(WebContext ac);
    }

    /// <summary>
    /// To filter after executing a procedure
    /// </summary>
    public interface IAfter
    {
        void Do(WebContext ac);
    }

    /// <summary>
    /// To filter asynchronously after executing a procedure
    /// </summary>
    public interface IAfterAsync
    {
        Task DoAsync(WebContext ac);
    }
}