using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// The encapsulation of a web request/response exchange context.
	///
	/// buffer pooling -- reduces GC overhead when dealing with asynchronous request/response I/O
	///
	public class WebContext : IDisposable
	{
		private readonly HttpContext _impl;

		private readonly WebRequest _request;

		private readonly WebResponse _response;

		internal WebContext(HttpContext impl)
		{
			_impl = impl;
			_request = new WebRequest(impl.Request);
			_response = new WebResponse(impl.Response);
		}

		public ISession Session => _impl.Session;

		public WebRequest Request => _request;

		public WebResponse Response => _response;

		public WebSub Controller { get; }

		public WebAction Action { get; }

		public IZone Zone { get; internal set; }

		public IToken Token { get; }


		internal Task SendAsyncTask()
		{

			return _response.SendAsyncTask();
		}

		public void Dispose()
		{
		}
	}
}