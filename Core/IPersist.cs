namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistable object that provides conventional data exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource sc, ushort x = 0xffff);

        void Save<R>(ISink<R> sk, ushort x = 0xffff) where R : ISink<R>;
    }
}