using System;

namespace Greatbone.Core
{
    public interface IContent
    {
        ///
        /// The Internet Media Type (MIME) type.
        ///
        string MType { get; }

        ///
        /// If the content is in binary form (bytes)
        ///
        bool Senable { get; }

        ///
        /// If the buffer is rented from pool so as to return back lately.
        ///
        bool Poolable { get; }

        /// 
        /// The byte buffer that contains binary content.
        ///
        byte[] ByteBuffer { get; }

        ///
        /// The char buffer that contains text content.
        ///
        char[] CharBuffer { get; }

        /// 
        /// The actual number of bytes/chars, that is either a count or length of the content.
        ///
        int Size { get; }

        ///
        /// If any, the last modified time.
        ///
        DateTime? Modified { get; }
    }
}