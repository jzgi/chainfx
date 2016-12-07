namespace Greatbone.Core
{
    ///
    /// Represents a data object that follows certain input/ouput conventions.
    ///
    public interface IDat
    {
        void Load(ISource src, byte z = 0);

        void Dump<R>(ISink<R> snk, byte z = 0) where R : ISink<R>;
    }
}
