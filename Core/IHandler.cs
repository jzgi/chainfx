namespace Greatbone.Core
{
    ///
    /// A handler method, such as an action or event.
    ///
    public interface IHandler : IKeyed<string>
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