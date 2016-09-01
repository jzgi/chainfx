using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace Greatbone.Core
{
    public class WebRequest : DefaultHttpRequest
    {
        private readonly HttpRequest _impl;

        private object data;

        public WebRequest(HttpContext context) : base(context)
        {
        }

        public T Json<T>()
        {
            if (data == null)
            {
            }
            return (T) data;
        }

//		public async ReadBodyAsync()
//		{
//			long? len = _impl.ContentLength;
//			string ctype = _impl.ContentType;
//			byte[] _buffer;
//			if ("application/json".Equals(ctype))
//			{
//				// get a pooled byte buffer
//				_buffer = BufferPool.Lease((int) len.Value);
//			}
//			if ("application/json".Equals(ctype))
//			{
//				_buffer = BufferPool.Lease((int) len.Value);
//			}
//
//			if (_buffer == null)
//			{
//			}
//			await _impl.Body.ReadAsync(_buffer, 0, _buffer.Length);
//		}


//		public T Object<T>()
//		{
//			JsonSerializer ser = new JsonSerializer();
//			JsonReader reader = new JsonTextReader(new Utf8TextReader(_buffer, 0, 0));
//			return ser.Deserialize<T>(reader);
//		}

        internal class Utf8TextReader : TextReader
        {
            internal Utf8TextReader(byte[] buf, int offset, int count)
            {
            }
        }
    }
}