namespace Greatbone.Core
{
    ///
    /// A resultset returned from query, that provides data access mechanisms.
    ///
    public interface IResultSet : ISource
    {
        bool NextRow();

        bool NextResult();

        D ToDat<D>(byte flags = 0) where D : IDat, new();

        D[] ToDats<D>(byte bits = 0) where D : IDat, new();
    }
}