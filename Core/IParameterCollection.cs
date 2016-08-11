using System;

namespace Greatbone.Core
{
	public interface IParameterCollection
	{
		void Put(string name, int value);

		void Put(string name, decimal value);

		void Put(string name, string value);

		void Put(string name, ArraySegment<byte> value);
	}
}