using System;
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

		public bool Read(string name, ref short value)
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

		public bool Read(string name, ref DateTime value)
		{
			throw new NotImplementedException();
		}

		public bool Read(string name, ref string value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref string[] value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref bool value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read<K, V>(string name, ref Dictionary<K, V> value)
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

		public void Write(string name, bool value)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, short value)
		{
			throw new NotImplementedException();
		}

		public void Write(string name, DateTime value)
		{
			throw new NotImplementedException();
		}

		public void Write<T>(string name, List<T> list)
		{
			throw new System.NotImplementedException();
		}

		public void Write<V>(string name, Dictionary<string, V> dict)
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