namespace Greatbone.Core
{
    ///
    /// A data object that follows certain input/ouput conventions.
    ///
    public interface IDat
    {
        void Load(ISource src, byte bits = 0);

        void Dump<R>(ISink<R> snk, byte bits = 0) where R : ISink<R>;
    }
}
