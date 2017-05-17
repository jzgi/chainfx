namespace Greatbone.Core
{
    public interface IContent
    {
        ///
        /// The Internet Media Type (MIME) type, as the value of Content-Type header.
        ///
        string Type { get; }

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