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

        P ToObj<P>(uint x = 0) where P : IPersist, new();

        P[] ToArr<P>(uint x = 0) where P : IPersist, new();
    }
}