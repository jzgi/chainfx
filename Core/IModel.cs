namespace Greatbone.Core
{
    ///
    /// Represents a content model object.
    ///
    public interface IModel
    {
        ///
        /// This includes multiple objects.
        ///
        bool Single { get; }

        ///
        /// Dump to the given sink.
        ///
        void WriteData<R>(IDataOutput<R> snk) where R : IDataOutput<R>;

        ///
        /// Dump as specified content.
        ///
        C Dump<C>() where C : IContent, IDataOutput<C>, new();
    }
}