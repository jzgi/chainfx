using System.Collections.Generic;

namespace Greatbone.Core
{
	public static class ISerialReaderExtensions
	{
		public static T Read<T>(this ISerialReader r) where T : ISerial, new()
		{
			T obj = new T();

//			r.ReadStart();
			obj.ReadFrom(r);
//			r.ReadEnd();

			return obj;
		}

		public static List<T> ReadArray<T>(this ISerialReader r) where T : ISerial, new()
		{
			List<T> lst = new List<T>(64);
//			if (!r.ReadArrayStart()) return lst;


			T obj = new T();
//			r.ReadStart();
			obj.ReadFrom(r);
//			r.ReadEnd();
			return null;
		}
	}
}