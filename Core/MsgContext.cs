using System;
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

		ISerial body;

		internal MsgContext(HttpContext context)
		{

		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}