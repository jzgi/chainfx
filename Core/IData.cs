namespace Greatbone.Core
{
    ///
    /// Represents a data object that follows certain input/ouput conventions.
    ///
    public interface IData
    {
        void Load(ISource src, byte bits = 0);

        void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>;
    }
}
