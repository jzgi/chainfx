namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistent data object that is compliant to standard data exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource c, int x);

        void Save<R>(ISink<R> k, int x) where R : ISink<R>;
    }
}