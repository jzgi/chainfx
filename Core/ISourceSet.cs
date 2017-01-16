namespace Greatbone.Core
{
    ///
    /// A DAT source or result set consisting of many source objects.
    ///
    public interface ISourceSet : ISource
    {
        bool Next();

        D[] ToDats<D>(byte flags = 0) where D : IDat, new();
    }
}