namespace Greatbone.Core
{
    ///
    /// Represents a content model object.
    ///
    public interface IModel
    {
        ///
        /// This model includes multiple data items.
        ///
        bool Multi { get; }

        ///
        /// Write data into the given output object.
        ///
        void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>;

        ///
        /// Dump as specified content.
        ///
        C Dump<C>() where C : IContent, IDataOutput<C>, new();
    }
}