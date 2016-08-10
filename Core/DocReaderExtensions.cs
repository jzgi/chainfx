using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class DocReaderExtensions
	{
		public static T Read<T>(this IDocReader r) where T : IDoc, new()
		{
			T obj = new T();
			obj.From(r);
			return obj;
		}

		public static List<T> ReadArray<T>(this IDocReader r) where T : IDoc, new()
		{
			List<T> lst = new List<T>(64);
			if (!r.ReadArrayStart()) return lst;


			T obj = new T();
			obj.From(r);
			return null;
		}
	}
}