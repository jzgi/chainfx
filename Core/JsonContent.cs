using System.Collections.Generic;

namespace Greatbone.Core
{
	public class JsonContent : DynamicContent, IReader, IWriter
	{
		public JsonContent(byte[] buffer) : base(buffer)
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

		public bool Read<T>(string name, ref T value) where T : ISerial
		{
			throw new System.NotImplementedException();
		}

		public bool Read<T>(string name, ref List<T> value) where T : ISerial
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref List<string> value)
		{
			throw new System.NotImplementedException();
		}

		public bool Read(string name, ref string[] value)
		{
			throw new System.NotImplementedException();
		}

		public void Write(string name, int value)
		{
			Put('"');
			Put(name);
			Put('"');
			Put(':');
			Put(value);
		}

		public void Write(string name, decimal value)
		{
			Put('"');
			Put(name);
			Put('"');
			Put(':');
			Put(value);
		}

		public void Write(string name, string value)
		{
			Put('"');
			Put(name);
			Put('"');
			Put(':');
			Put(value);
		}

		public void Write(string name, ISerial value)
		{
			Put('"');
			Put(name);
			Put('"');
			Put(':');

			Put('{');
			value.To(this);
			Put('}');
		}

		public void Write(string name, List<ISerial> list)
		{
			Put('"');
			Put(name);
			Put('"');
			Put(':');

			Put('[');
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (i > 0)
					{
						Put(',');
					}

					ISerial obj = list[i];

					Put('{');
					obj.To(this);
					Put('}');
				}
			}
			Put(']');
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
			Put('"');
			Put(name);
			Put('"');
			Put(':');

			Put('[');
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i > 0)
					{
						Put(',');
					}

					ISerial obj = array[i];

					Put('{');
					obj.To(this);
					Put('}');
				}
			}
			Put(']');
		}
	}
}