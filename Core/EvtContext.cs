using System;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// An event queue message
	///
	public class EvtContext : IDisposable
	{
		private string topic;

		private string skey;

		private string subkey;

		ISerial body;

		internal EvtContext(HttpContext context)
		{

		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}