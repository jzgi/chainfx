using System;

namespace Greatbone.Core
{

    public interface IContent
    {

        /// <summary>
        /// Content-Type
        /// </summary>
        ///
        string Type { get; }

        /// <summary>
        /// The content is in raw bytes.
        /// </summary>
        ///
        bool IsRaw { get; }


        bool IsPooled { get; }

        /// <summary
        /// >The byte buffer that contains octet content.
        /// </summary>
        ///
        byte[] ByteBuffer { get; }

        /// <summary
        /// >The char buffer that contains text content.
        /// </summary>
        ///
        char[] CharBuffer { get; }

        /// <summary>
        /// The actual number of bytes/chars.
        /// </summary>
        ///
        int Size { get; }

        DateTime? Modified { get; }

    }

}