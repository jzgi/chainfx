using System.Collections.Generic;

namespace Greatbone.Core
{
	///
	/// DataReader/ParameterCollection, JSON or binary
	public interface IDocWriter
	{
		void WriteArrayStart();

		void WriteArrayEnd();

		void WriteStart();

		void WriteEnd();

		void WriteSep();

		void Write(string name, int value);

		void Write(string name, decimal value);

		void Write(string name, string value);

		void Write<T>(string name, List<T> value) where T : IDoc;

	}
}