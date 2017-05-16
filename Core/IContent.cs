namespace Greatbone.Core
{
    public interface IContent
    {
        ///
        /// The Internet Media Type (MIME) type, as the value of Content-Type header.
        ///
        string Type { get; }

        ///
        /// If the content is in binary form hence able to send asynchronously.
        ///
        bool Octal { get; }

        ///
        /// Can the content buffer be rented from pool so as to returned back lately.
        ///
        bool Poolable { get; }

        /// 
        /// The byte buffer that contains binary content. Can be null.
        ///
        byte[] ByteBuffer { get; }

        ///
        /// The char buffer that contains text content. Can be null.
        ///
        char[] CharBuffer { get; }

        /// 
        /// This is either the count or length of the content.
        ///
        int Size { get; }
    }
}