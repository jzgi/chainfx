namespace Greatbone.Core
{
    ///
    /// Represents a content object model.
    ///
    public interface IModel
    {
        void Dump<R>(ISink<R> snk) where R : ISink<R>;

        IContent Dump();
    }
}