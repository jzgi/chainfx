namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistable object that is compliant to standard data exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource sc, int x = -1);

        void Save<R>(ISink<R> sk, int x = -1) where R : ISink<R>;
    }
}