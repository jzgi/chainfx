using System;

namespace Greatbone.Core
{
    public interface IContent
    {
        ///
        /// The Content-Type.
        ///
        string CType { get; }

        ///
        /// If the content is in binary form (bytes)
        ///
        bool IsBinary { get; }

        ///
        /// If the buffer is rented from pool so as to return back lately.
        ///
        bool IsPoolable { get; }

        /// 
        /// The byte buffer that contains binary content.
        ///
        byte[] ByteBuf { get; }

        ///
        /// The char buffer that contains text content.
        ///
        char[] CharBuf { get; }

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