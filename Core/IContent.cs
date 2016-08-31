using System;

namespace Greatbone.Core
{
	public interface IContent
	{
		/// <summary>Content-Type</summary>
		///
		string Type { get; }

		/// <summary>The byte buffer that contains the content.</summary>
		///
		byte[] Buffer { get; }


		/// <summary>The number of bytes.</summary>
		///
		int Count { get; }

		/// <summary>Time that was last modified.</summary>
		DateTime LastModified { get; }

		///
		/// <summary>A computed ETag value.</summary>
		///
		long ETag { get; }
	}
}