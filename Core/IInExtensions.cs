using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class IInExtensions
    {
        public static T Read<T>(this IDataIn i) where T : IData, new()
        {
            T obj = new T();
          
            return obj;
        }

        public static List<T> ReadArray<T>(this IDataIn r) where T : IData, new()
        {
            List<T> lst = new List<T>(64);
            //			if (!r.ReadArrayStart()) return lst;


            T obj = new T();
            //			r.ReadStart();
            obj.In(r);
            //			r.ReadEnd();
            return null;
        }
    }
}