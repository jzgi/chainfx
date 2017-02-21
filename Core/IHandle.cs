namespace Greatbone.Core
{
    ///
    /// A handler method, either an action or an event.
    ///
    public interface IHandle : IRollable
    {
        ///
        /// Whether this handler method is async.
        ///
        bool Async { get; }

        ///
        /// Whether this handler method has an optional argument.
        ///
        bool Arg { get; }
    }
}