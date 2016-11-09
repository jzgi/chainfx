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

        B ToBean<B>(byte z = 0) where B : IBean, new();

        B[] ToBeans<B>(byte z = 0) where B : IBean, new();

    }
    
}