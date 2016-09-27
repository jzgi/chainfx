using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class IOutputExtensions
	{
		public static void Write(this IOutput w, params IDat[] arr)
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

		public static void Write<T>(this IOutput w, IList<T> coll) where T : IDat
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