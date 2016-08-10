using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Greatbone.Core
{
	///
	/// The wrapper of a HTTP response, providing efficient output methods and cache control.
	///
	public class WebResponse
	{
		// the underlying implementation of a response
		private readonly HttpResponse _impl;

		private IContent _content;

		// byte-wise etag checksum, for text-based output only
		private ulong _checksum;

		internal WebResponse(HttpResponse impl)
		{
			_impl = impl;
		}

		public IContent Content
		{
			get { return _content; }
			set { _content = value; }
		}

		public void SetCacheControl()
		{
		}


		public void SetJson(object obj)
		{
			string text = JsonConvert.SerializeObject(obj);

			byte[] bytes = Encoding.UTF8.GetBytes(text);
			Content = new DummyContent
			{
				Type = "application/json",
				Buffer = bytes,
				Offset = 0,
				Count = bytes.Length
			};
		}

		public void SetJson(JObject obj)
		{
		}

		public void SetJson(JArray arr)
		{
		}

		public int StatusCode
		{
			get { return _impl.StatusCode; }
			set { _impl.StatusCode = value; }
		}

		public void Redirect(string location)
		{
			_impl.Redirect(location);
		}

		public void Redirect(string location, bool permanent)
		{
			_impl.Redirect(location, permanent);
		}

		internal Task SendAsyncTask()
		{
			if (_content != null)
			{
				_impl.ContentLength = _content.Count;
				_impl.ContentType = _content.Type;

				// etag

				//
				return _impl.Body.WriteAsync(_content.Buffer, _content.Offset, _content.Count);
			}
			return null;
		}
	}

	class DummyContent : IContent
	{
		public string Type { get; set; }

		public byte[] Buffer { get; set; }

		public int Offset { get; set; }

		public int Count { get; set; }
	}

	class ResponseCachePolicy
	{
	}
}