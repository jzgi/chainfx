namespace Greatbone.Core
{
    /// <summary>
    /// Represents a content that is in format of either bytes or chars. Note that only bytes can be sent directly. 
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
        byte[] ByteBuffer { get; }

        ///
        /// The char buffer that contains text content. It can be null.
        ///
        char[] CharBuffer { get; }

        /// 
        /// This is either the count or length of the content.
        ///
        int Size { get; }

        /// 
        /// The ETag value for purpose of cache optimization. It can be null.
        /// 
        string ETag { get; }
    }
}