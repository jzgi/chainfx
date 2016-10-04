namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistent object that is compliant to standard exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource sc);

        void Save<R>(ISink<R> sk) where R : ISink<R>;
    }
}