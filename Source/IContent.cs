namespace ChainFx
{
    /// <summary>
    /// Represents a binary content. 
    /// </summary>
    public interface IContent
    {
        ///
        /// The Internet Media Type (MIME) type, as the value of Content-Type header.
        ///
        string CType { get; }

        ///
        /// The byte buffer that contains binary content. It's value can be null.
        ///
        byte[] Buffer { get; }

        /// 
        /// This is either the count or length of the content.
        ///
        int Count { get; }

        /// 
        /// The ETag value for cache optimization. It's value can be null.
        /// 
        string ETag { get; }
    }
}