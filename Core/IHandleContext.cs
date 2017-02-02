using System.Data;

namespace Greatbone.Core
{
    ///
    /// A handler method, such as an action or event.
    ///
    public interface IHandleContext<out H> where H : IHandle
    {
        WebServiceContext ServiceContext { get; }

        WebFolder Folder { get; }

        H Handle { get; }

        DbContext NewDbContext(IsolationLevel level = IsolationLevel.Unspecified);
    }
}