using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace Greatbone.Core
{
	public class WebRequest
	{
		private readonly WebController _controller;

		private readonly HttpRequest _impl;

		private byte[] _buffer;

		private int _count;

		private object _data;

		private IDataInput _input;

		internal WebRequest(WebController controller, HttpRequest impl)
		{
			_controller = controller;
			_impl = impl;
		}


		internal Task<int> ReadAsyncTask()
		{
			string ctype = _impl.ContentType;
			if ("application/json".Equals(ctype))
			{
				// get a pooled byte buffer
				_buffer = _controller.Service.Lease(false);
			}
			if ("application/json".Equals(ctype))
			{
				_buffer = _controller.Service.Lease(true);
			}
			return _impl.Body.ReadAsync(_buffer, 0, _buffer.Length);
		}

		public IFormCollection Form => _impl.Form;

		public IQueryCollection Query => _impl.Query;


		public JObject Json()
		{
			string s = "";
			JObject obj = JObject.Parse(s);
			return obj;
		}

		public T Object<T>()
		{


			JsonSerializer ser = new JsonSerializer();
			JsonReader reader = new JsonTextReader(new Utf8TextReader(_buffer, 0, 0));
			return ser.Deserialize<T>(reader);
		}

		internal class Utf8TextReader : TextReader
		{

			internal Utf8TextReader(byte[] buf, int offset, int count)
			{

			}
		}
	}
}