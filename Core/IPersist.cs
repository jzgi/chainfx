namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistable object that provides conventional data exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource s, byte x = 0xff);

        void Save<R>(ISink<R> s, byte x = 0xff) where R : ISink<R>;
    }
}