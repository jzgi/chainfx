using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
	/// <summary>
	/// A serialized content reader.
	/// </summary>
	public interface ISerialReader
	{
		bool Read(string name, ref bool value);

		bool Read(string name, ref short value);

		bool Read(string name, ref int value);

		bool Read(string name, ref decimal value);

		bool Read(string name, ref DateTime value);

		bool Read(string name, ref string value);

		bool Read<T>(string name, ref T value) where T : ISerial;

		bool Read(string name, ref List<string> value);

		bool Read(string name, ref string[] value);

		bool Read<T>(string name, ref List<T> value) where T : ISerial;

		bool Read<K, V>(string name, ref Dictionary<K, V> value);
	}
}