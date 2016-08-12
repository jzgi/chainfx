using System.Collections.Generic;

namespace Greatbone.Core
{
	public interface IReader
	{
		bool Read(string name, ref int value);

		bool Read(string name, ref decimal value);

		bool Read(string name, ref string value);

		bool Read<T>(string name, ref T value) where T : ISerial;

		bool Read<T>(string name, ref List<T> value) where T : ISerial;
	}
}