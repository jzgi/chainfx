namespace Greatbone.Core
{

    ///
    ///<summary>
    /// A resultset returned from query, that provides data access mechanisms.
    /// </summary>
    public interface IResultSet : ISource
    {
        bool NextRow();

        bool NextResult();

        P ToObj<P>(byte x = 0xff) where P : IPersist, new();

        P[] ToArr<P>(byte x = 0xff) where P : IPersist, new();

    }
}