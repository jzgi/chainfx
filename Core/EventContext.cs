using System;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// A message queue message
	///
	public class EventContext : IDisposable
	{
		private string topic;

		private string filter;

		ISerial @object;

		internal EventContext(HttpContext context)
		{

		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}