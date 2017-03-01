using System.Data;

namespace Greatbone.Core
{
    ///
    /// An execution context related to a handler method.
    ///
    public interface IDoerContext<out D> where D : IDoer
    {
        Service Service { get; }

        Folder Folder { get; }

        D Doer { get; }

        DbContext NewDbContext(IsolationLevel? level = null);
    }
}