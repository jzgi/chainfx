namespace Greatbone.Core
{
    ///
    /// A resultset returned from query, that provides data access mechanisms.
    ///
    public interface IResultSet : ISource
    {
        bool NextRow();

        bool NextResult();

        D ToObject<D>(byte flags = 0) where D : IData, new();

        D[] ToArray<D>(byte bits = 0) where D : IData, new();
    }
}