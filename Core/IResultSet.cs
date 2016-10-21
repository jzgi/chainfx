namespace Greatbone.Core
{
    ///
    ///<summary>
    /// A resultset returned from query execution. Used to separate data process logic.
    /// </summary>
    public interface IResultSet : ISource
    {
        bool NextRow();

        bool NextResult();

        T ToObj<T>(uint x = 0) where T : IPersist, new();

        T[] ToArr<T>(uint x = 0) where T : IPersist, new();
    }
}