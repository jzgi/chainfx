namespace Greatbone.Core
{
    ///
    /// Represents a content model object.
    ///
    public interface IModel
    {
        ///
        /// Dump to the given sink.
        ///
        void Dump<R>(ISink<R> snk) where R : ISink<R>;

        ///
        /// Dump as specified content.
        ///
        C Dump<C>() where C : IContent, ISink<C>, new();
    }
}