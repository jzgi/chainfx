using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// An event queue message
	///
	public class MsgContext : IDisposable
	{
		private string topic;

		private string key;

		HttpResponseMessage msg;

		internal MsgContext(HttpContext context)
		{

		}
		

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}