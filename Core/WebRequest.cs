using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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


		public JObject JsonObject()
		{
			string s = "";
			JObject obj = JObject.Parse(s);
			return obj;
		}
	}
}