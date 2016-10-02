namespace Greatbone.Core
{
    /// <summary>
    /// Represents a persistent object that is compliant to standard exchange mechanisms.
    /// </summary>
    public interface IPersist
    {
        void Load(ISource sc, int x);

        void Save<R>(ISink<R> sk, int x) where R : ISink<R>;
    }
}