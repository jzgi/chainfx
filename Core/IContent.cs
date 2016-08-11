using System;

namespace Greatbone.Core
{
	public interface IContent
	{
		///
		/// Content-Type
		///
		string Type { get; }

		///
		/// The byte buffer that contains the content.
		///
		byte[] Buffer { get; }


		///
		/// The number of bytes
		///
		int Count { get; }

		DateTime LastModified { get; }

		///
		/// A computed ETag value
		///
		long ETag { get; }
	}
}