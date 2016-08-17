using System;

namespace Greatbone.Core
{
	public interface IParameterCollection
	{
		void Add(string name, int value);

		void Add(string name, decimal value);

		void Add(string name, string value);

		void Add(string name, ArraySegment<byte> value);
	}
}