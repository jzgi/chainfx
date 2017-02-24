using System.Data;

namespace Greatbone.Core
{
    ///
    /// An execution context related to a handler method.
    ///
    public interface IHandlerContext<out H> where H : IHandler
    {
        ServiceContext ServiceContext { get; }

        Folder Folder { get; }

        H Handler { get; }

        DbContext NewDbContext(IsolationLevel? level = null);
    }
}