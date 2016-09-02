using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Greatbone.Core
{
    /// <summary>
    /// The resources and operations of a web request.
    ///</summary>
    /// <remarks>Asynchronou reading are carefully designed. </remarks>
    public class WebRequest : DefaultHttpRequest
    {
        private byte[] buffer;

        private int count;

        // the parsed content, that is a deserialized object or bytes
        private object content;

        public WebRequest(HttpContext ctx) : base(ctx)
        {
        }

        /// <summary>
        /// Receiveds request body into a buffer held by the buffer field.
        ///</summary>
        internal async void ReceiveAsync()
        {
            long? clen = ContentLength;
            if (buffer == null && clen > 0)
            {
                int len = (int) clen.Value;
                // borrow a byte array from the pool
                buffer = BufferPool.Lease(len);
                count = len;
                await Body.ReadAsync(buffer, 0, len);
            }
        }

        public ArraySegment<byte> ByteArray()
        {
            if (buffer == null)
            {
                ReceiveAsync();
            }
            return new ArraySegment<byte>(buffer, 0, count);
        }

        public T Object<T>() where T : ISerial, new()
        {
            if (content == null)
            {
                if (buffer == null)
                {
                    ReceiveAsync();
                }
                // init content
                ISerialReader reader = null;
                string ctype = ContentType;
                long? clen = ContentLength;
                if ("application/json".Equals(ctype))
                {
                    reader = new JsonContent(buffer, (int) clen);
                }
                else if ("application/bjson".Equals(ctype))
                {
                    reader = new BJsonContent(buffer, (int) clen);
                }
                content = reader.Read<T>();
            }
            return (T) content;
        }

        internal class Utf8TextReader : TextReader
        {
            internal Utf8TextReader(byte[] buf, int offset, int count)
            {
            }
        }
    }
}