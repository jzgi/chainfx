using System.Collections;
using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class DocWriterExtensions
	{
		public static void Write(this IDocWriter w, params IDoc[] arr)
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

		public static void Write(this IDocWriter w, ICollection<IDoc> coll)
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