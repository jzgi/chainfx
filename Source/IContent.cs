namespace SkyChain
{
    /// <summary>
    /// Represents a binary content ready to send through web context. 
    /// </summary>
    public interface IContent
    {
        ///
        /// The Internet Media Type (MIME) type, as the value of Content-Type header.
        ///
        string Type { get; }

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