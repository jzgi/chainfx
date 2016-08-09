using System.Collections.Generic;

namespace Greatbone.Core
{
	public class JsonOutput : IDataOutput
	{
		public void PutStart()
		{
			throw new System.NotImplementedException();
		}

		public void PutEnd()
		{
			throw new System.NotImplementedException();
		}

		public void Put(string name, int value)
		{
			throw new System.NotImplementedException();
		}

		public void Put(string name, decimal value)
		{
			throw new System.NotImplementedException();
		}

		public void Put(string name, string value)
		{
			throw new System.NotImplementedException();
		}

		public void Put<T>(string name, List<T> value) where T : IData
		{
			throw new System.NotImplementedException();
		}
	}
}