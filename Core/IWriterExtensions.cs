using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class IWriterExtensions
	{
		public static void Write(this IWriter w, params ISerial[] arr)
		{
			w.WriteArrayStart();
			for (int i = 0; i < arr.Length; i++)
			{
				if (i > 0)
				{
					w.WriteSep();
				}
				arr[i].To(w);
			}
			w.WriteArrayEnd();
		}

		public static void Write(this IWriter w, ICollection<ISerial> coll)
		{
			w.WriteArrayStart();
			int i = 0;
			foreach (var o in coll)
			{
				if (i++ > 0)
				{
					w.WriteSep();
				}
				o.To(w);
			}
			w.WriteArrayEnd();
		}
	}
}