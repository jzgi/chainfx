using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Primitives;

namespace Greatbone.Core
{
    /// <summary>
    /// The resources and operations of a web request.
    ///</summary>
    /// <remarks>Asynchronou reading are carefully designed. </remarks>
    public class WebRequest : DefaultHttpRequest
    {

        // in raw bytes
        private byte[] buffer;

        private int count;

        // the content is parsed on demand of the application
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
                int len = (int)clen.Value;
                // borrow a byte array from the pool
                buffer = BufferPool.Lease(len);
                count = await Body.ReadAsync(buffer, 0, len);
            }
        }

        public ArraySegment<byte> Bytes
        {
            get
            {
                if (buffer == null)
                {
                    ReceiveAsync();
                }
                return new ArraySegment<byte>(buffer, 0, count);
            }
        }

        public ISerialReader Reader
        {
            get
            {
                if (content == null)
                {
                    if (buffer == null)
                    {
                        ReceiveAsync();
                    }
                    // init content
                    string ctype = ContentType;
                    long? clen = ContentLength;
                    if ("application/bjson".Equals(ctype))
                    {
                        return new JsonbContent(buffer, (int)clen);
                    }
                    else
                    {
                        return new JsonContent(buffer, (int)clen);
                    }
                }
                return null;
            }
        }

        public T Serial<T>() where T : ISerial, new()
        {
            ISerialReader r = this.Reader;
            T o = new T();
            r.Read(ref o);
            return o;
        }

        public bool GetParam(string name, ref int value)
        {
            StringValues values;
            if (Query.TryGetValue(name, out values))
            {
                string v = values[0];
                int i;
                if (Int32.TryParse(v, out i))
                {
                    value = i;
                    return true;
                }
            }
            return false;
        }

        public bool GetParam(string name, ref string value)
        {
            StringValues values;
            if (Query.TryGetValue(name, out values))
            {
                value = values[0];
                return true;
            }
            return false;
        }


        public bool GetField(string name, ref int value)
        {
            StringValues values;
            if (Form.TryGetValue(name, out values))
            {
                string v = values[0];
                int i;
                if (Int32.TryParse(v, out i))
                {
                    value = i;
                    return true;
                }
            }
            return false;
        }

        public bool GetField(string name, ref string value)
        {
            StringValues values;
            if (Form.TryGetValue(name, out values))
            {
                value = values[0];
                return true;
            }
            return false;
        }


    }
}