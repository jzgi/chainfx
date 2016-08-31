using System;
using Greatbone.Core;

namespace Greatbone.Sample
{
	public struct Msg : ISerial
	{
		internal int id;

		internal short subtype;

		internal string from;

		internal string to;

		internal string content;

		internal DateTime time;

		public void ReadFrom(ISerialReader r)
		{
			r.Read(nameof(id), ref id);
			r.Read(nameof(subtype), ref subtype);
			r.Read(nameof(from), ref from);
			r.Read(nameof(to), ref to);
			r.Read(nameof(content), ref content);
			r.Read(nameof(time), ref time);
		}

		public void WriteTo(ISerialWriter w)
		{
			w.Write(nameof(id),id);
			w.Write(nameof(subtype), subtype);
			w.Write(nameof(from), from);
			w.Write(nameof(to), to);
			w.Write(nameof(content), content);
			w.Write(nameof(time), time);
		}
	}
}