using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// The encapsulation of a web request/response exchange context.
	///
	public class WebContext : IDisposable
	{
		private HttpContext _impl;

		private readonly WebRequest _request;

		private readonly WebResponse _response;

		internal WebContext(HttpContext impl)
		{
			_impl = impl;
			_request = new WebRequest(impl.Request);
			_response = new WebResponse(impl.Response);
		}

		public WebRequest Request => _request;

		public WebResponse Response => _response;

		public WebSub Controller { get; internal set; }

		public WebAction Action { get; internal set; }

		public IZone Zone { get; internal set; }

		public IToken Token { get; internal set; }


		internal Task SendAsyncTask()
		{
			return _response.SendAsyncTask();
		}

		public void Dispose()
		{
		}
	}
}