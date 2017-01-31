namespace Greatbone.Core
{
    ///
    /// Represents a content model object.
    ///
    public interface IModel
    {
        ///
        /// Write a single (or current) data entry into the given output object.
        ///
        void WriteData<R>(IDataOutput<R> o) where R : IDataOutput<R>;

        ///
        /// Dump as specified content.
        ///
        C Dump<C>() where C : IContent, IDataOutput<C>, new();

        ///
        /// If this model includes multiple data entries.
        ///
        bool DataSet { get; }

        ///
        /// Move to next data entry.
        ///
        bool Next();
    }
}