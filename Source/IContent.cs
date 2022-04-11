namespace Chainly
{
    /// <summary>
    /// Represents a binary web content. 
    /// </summary>
    public interface IContent
    {
        ///
        /// The Internet Media Type (MIME) type, as the value of Content-Type header.
        ///
        string CType { get; }

        ///
        /// The byte buffer that contains binary content. It can be null.
        ///
        byte[] Buffer { get; }

        /// 
        /// This is either the count or length of the content.
        ///
        int Count { get; }

        /// 
        /// The ETag value for purpose of cache optimization. It can be null.
        /// 
        string ETag { get; }
    }
}