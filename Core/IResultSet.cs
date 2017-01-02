namespace Greatbone.Core
{
    ///
    /// A resultset returned from query, that provides data access mechanisms.
    ///
    public interface IResultSet : ISource
    {
        bool NextRow();

        bool NextResult();

        D ToDataObj<D>(byte z = 0) where D : IData, new();

        D[] ToDataArr<D>(byte bits = 0) where D : IData, new();
    }
}