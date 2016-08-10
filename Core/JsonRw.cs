using System.Collections.Generic;

namespace Greatbone.Core
{
	public class JsonRw : ContentRw, IDocReader, IDocWriter
	{
		public bool ReadArrayStart()
		{
			throw new System.NotImplementedException();
		}

		public bool ReadArrayEnd()
		{
			throw new System.NotImplementedException();
		}

		public bool ReadSep()
		{
			throw new System.NotImplementedException();
		}

		public bool ReadStart()
		{
			throw new System.NotImplementedException();
		}

		public bool ReadEnd()
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref int value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref decimal value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref string value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read<T>(string name, ref List<T> value) where T : IDoc
		{
			throw new System.NotImplementedException();
		}

		public void WriteArrayStart()
		{
			Put('[');
		}

		public void WriteArrayEnd()
		{
			Put(']');
		}

		public void WriteStart()
		{
			throw new System.NotImplementedException();
		}

		public void WriteEnd()
		{
			throw new System.NotImplementedException();
		}

		public void WriteSep()
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, int value)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, decimal value)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, string value)
		{
			throw new System.NotImplementedException();
		}

		public void Write<T>(string name, List<T> value) where T : IDoc
		{
			throw new System.NotImplementedException();
		}
	}
}