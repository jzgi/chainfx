using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class ISerialReaderExtensions
    {
        public static T Read<T>(this ISerialReader r) where T : ISerial, new()
        {
            T obj = new T();
            r.Object(() =>
            {
                obj.From(r);
            });
            return obj;
        }

        public static List<T> ReadArray<T>(this ISerialReader r) where T : ISerial, new()
        {
            List<T> lst = new List<T>(64);
            //			if (!r.ReadArrayStart()) return lst;


            T obj = new T();
            //			r.ReadStart();
            obj.From(r);
            //			r.ReadEnd();
            return null;
        }
    }
}