using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class IOutExtensions
	{
		public static void Write(this IOut w, params IData[] arr)
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (i > 0)
				{
//					w.WriteSep();
				}
				arr[i].Write(w);
			}
		}

		public static void Write<T>(this IOut w, IList<T> coll) where T : IData
		{
//			w.WriteArrayStart();
			int i = 0;
			foreach (var o in coll)
			{
				if (i++ > 0)
				{
//					w.WriteSep();
				}
				o.Write(w);
			}
//			w.WriteArrayEnd();
		}
	}
}