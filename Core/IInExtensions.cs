using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class IInExtensions
    {
        public static T Read<T>(this IIn i) where T : IData, new()
        {
            T obj = new T();
          
            return obj;
        }

        public static List<T> ReadArray<T>(this IIn r) where T : IData, new()
        {
            List<T> lst = new List<T>(64);
            //			if (!r.ReadArrayStart()) return lst;


            T obj = new T();
            //			r.ReadStart();
            obj.Read(r);
            //			r.ReadEnd();
            return null;
        }
    }
}