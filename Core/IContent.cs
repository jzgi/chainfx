using System;

namespace Greatbone.Core
{
    public interface IContent
    {
        ///
        /// The Content-Type.
        ///
        string MimeType { get; }

        ///
        /// If the content is in binary form (bytes)
        ///
        bool Binary { get; }

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
        /// The actual number of bytes/chars.
        ///
        int Size { get; }

        ///
        /// If any, the last modified time.
        ///
        DateTime? Modified { get; }
    }
}