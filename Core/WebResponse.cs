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

		// public, private, or no-cache
		private bool? _shared;

		// max age in seconds
		private int _maxage;

		// byte-wise etag checksum, for text-based output only
		private ulong _checksum;

		internal WebResponse(HttpResponse impl)
		{
			_impl = impl;
		}

		public IContent Content { get; set; }

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

		public bool? IsShared => _shared;

		public int MaxAge => _maxage;

		public void SetCachePolicy(bool? shared, int maxage)
		{
			_shared = shared;
			_maxage = maxage;
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
			if (Content != null)
			{
				_impl.ContentLength = Content.Count;
				_impl.ContentType = Content.Type;

				// etag

				//
				return _impl.Body.WriteAsync(Content.Buffer, Content.Offset, Content.Count);
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