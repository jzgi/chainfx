namespace Greatbone.Core
{
    /// <summary>
    /// The descriptor of a handler method that is either an action or an event.
    /// </summary>
    public interface IDoer : IMappable<string>
    {
        /// <summary>
        /// Whether this handler method is an async method.
        /// </summary>
        bool IsAsync { get; }

        /// <summary>
        /// Whether this handler method has a subscript parameter, which is the second parameter that must be Int32.
        /// </summary>
        bool HasSubscript { get; }

        /// <summary>
        /// A limit of records that this doer may deal with.
        /// </summary>
        int Limit { get; }
    }
}