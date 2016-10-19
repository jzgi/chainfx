namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistable object that provides conventional data exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource s, uint x = 0);

        void Save<R>(ISink<R> s, uint x = 0) where R : ISink<R>;
    }
}