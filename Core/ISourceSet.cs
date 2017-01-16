namespace Greatbone.Core
{
    ///
    /// A data source or result set consisting of many source objects.
    ///
    public interface ISourceSet : ISource
    {
        bool Next();

        D[] ToDatas<D>(byte flags = 0) where D : IData, new();
    }
}