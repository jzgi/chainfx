using System.Collections.Generic;

namespace Greatbone.Core
{
	public class JsonBinary : IDocInput, IDocOutput
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

		public bool Got<T>(string name, ref List<T> value) where T : IDoc
		{
			throw new System.NotImplementedException();
		}

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

		public void Put<T>(string name, List<T> value) where T : IDoc
		{
			throw new System.NotImplementedException();
		}
	}
}