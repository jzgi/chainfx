using System;
using Microsoft.AspNetCore.Http;

namespace Greatbone.Core
{
	///
	/// A message queue message
	///
	public class MsgContext : IDisposable
	{
		private string topic;

		private string filter;

		ISerial @object;

		internal MsgContext(HttpContext context)
		{

		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}