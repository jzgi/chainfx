using System.Collections.Generic;

namespace Greatbone.Core
{
	public class JsonInput : IDataInput
	{
		public bool GotStart()
		{
			throw new System.NotImplementedException();
		}

		public bool GotEnd()
		{
			throw new System.NotImplementedException();
		}

		public bool Got(string name, ref int value)
		{
			throw new System.NotImplementedException();
		}

		public bool Got(string name, ref decimal value)
		{
			throw new System.NotImplementedException();
		}

		public bool Got(string name, ref string value)
		{
			throw new System.NotImplementedException();
		}

		public bool Got<T>(string name, ref List<T> value) where T : IData
		{
			throw new System.NotImplementedException();
		}

	}
}