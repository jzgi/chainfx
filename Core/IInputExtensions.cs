using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class IInputExtensions
    {
        public static T Read<T>(this IInput i) where T : IDat, new()
        {
            T obj = new T();
          
            return obj;
        }

        public static List<T> ReadArray<T>(this IInput r) where T : IDat, new()
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