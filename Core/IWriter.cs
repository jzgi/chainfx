using System.Collections.Generic;

namespace Greatbone.Core
{
	///
	/// DataReader/ParameterCollection, JSON or binary
	public interface IWriter
	{
		void Write(string name, int value);

		void Write(string name, decimal value);

		void Write(string name, string value);

		void Write(string name, ISerial value);

		void Write(string name, List<ISerial> list);

		void Write(string name, params ISerial[] array);
	}
}