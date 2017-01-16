namespace Greatbone.Core
{
    ///
    /// A resultset returned from query, that provides data access mechanisms.
    ///
    public interface ISourceSet : ISource
    {
        bool Next();

        D[] ToDats<D>(byte flags = 0) where D : IDat, new();
    }
}