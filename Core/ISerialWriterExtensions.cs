using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class ISerialWriterExtensions
	{
		public static void Write(this ISerialWriter w, params ISerial[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (i > 0)
				{
//					w.WriteSep();
				}
				arr[i].To(w);
			}
		}

		public static void Write<T>(this ISerialWriter w, IList<T> coll) where T : ISerial
		{
//			w.WriteArrayStart();
			int i = 0;
			foreach (var o in coll)
			{
				if (i++ > 0)
				{
//					w.WriteSep();
				}
				o.To(w);
			}
//			w.WriteArrayEnd();
		}
	}
}