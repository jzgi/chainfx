using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class IOutExtensions
	{
		public static void Write<R>(this IDataOut<R> w, params IData[] arr) where R : IDataOut<R>
		{
			for (int i = 0; i < arr.Length; i++)
			{
				if (i > 0)
				{
//					w.WriteSep();
				}
				arr[i].Out(w);
			}
		}

		public static void Write<T, R>(this IDataOut<R> w, IList<T> coll) where T : IData where R : IDataOut<R>
		{
//			w.WriteArrayStart();
			int i = 0;
			foreach (var o in coll)
			{
				if (i++ > 0)
				{
//					w.WriteSep();
				}
				o.Out(w);
			}
//			w.WriteArrayEnd();
		}
	}
}