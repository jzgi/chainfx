using System.Data;

namespace Greatbone.Core
{
    /// <summary>
    /// An execution context for a handler method.
    /// </summary>
    /// <typeparam name="D">The descriptor type of the related doer method.</typeparam>
    public interface IDoerContext<out D> where D : IDoer
    {
        Service Service { get; }

        Work Work { get; }

        D Doer { get; }

        int Subscript { get; }

        DbContext NewDbContext(IsolationLevel? level = null);
    }
}