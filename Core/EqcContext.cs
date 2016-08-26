using System;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// An event queue message
	///
	public class EqcContext : IDisposable
	{
		private string topic;

		private string key;

		ISerial body;

		internal EqcContext(HttpContext context)
		{

		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}