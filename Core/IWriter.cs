using System.Collections.Generic;

namespace Greatbone.Core
{
	///
	/// DataReader/ParameterCollection, JSON or binary
	public interface IWriter
	{
		void Write(string name, bool value);

		void Write(string name, int value);

		void Write(string name, decimal value);

		void Write(string name, string value);

		void Write(string name, ISerial value);

		void Write<T>(string name, List<T> list);

		void Write<V>(string name, Dictionary<string, V> dict);

		void Write(string name, params string[] array);

		void Write(string name, params ISerial[] array);
	}
}