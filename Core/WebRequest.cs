using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Greatbone.Core
{
	public class WebRequest
	{
		private readonly HttpRequest _impl;

		private object data;

		internal WebRequest(HttpRequest impl)
		{
			_impl = impl;
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


		public IFormCollection Form => _impl.Form;

		public IQueryCollection Query => _impl.Query;


		public JObject Json()
		{
			string s = "";
			JObject obj = JObject.Parse(s);
			return obj;
		}

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