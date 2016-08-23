using System.Collections.Generic;

namespace Greatbone.Core
{
	///
	/// Binary JSON content.
	///
	public class BJsonContent : DynamicContent, IReader, IWriter
	{
		public BJsonContent(byte[] buffer) : base(buffer)
		{
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

		public bool Read(string name, ref string[] value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read<T>(string name, ref T value) where T : ISerial
		{
			throw new System.NotImplementedException();
		}

		public bool Read<T>(string name, ref List<T> value) where T : ISerial
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

		public void Write(string name, ISerial value)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, List<ISerial> list)
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref List<string> value)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, List<string> list)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, params string[] array)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, params ISerial[] array)
		{
			throw new System.NotImplementedException();
		}
	}
}