namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistable object that provides conventional data exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource sc, uint x = 0);

        void Save<R>(ISink<R> sk, uint x = 0) where R : ISink<R>;
    }
}