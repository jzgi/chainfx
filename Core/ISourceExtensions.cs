using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class ISourceExtensions
    {
        public static T Read<T>(this ISource i) where T : IPersist, new()
        {
            T obj = new T();
          
            return obj;
        }

        public static List<T> ReadArray<T>(this ISource r) where T : IPersist, new()
        {
            List<T> lst = new List<T>(64);
            //			if (!r.ReadArrayStart()) return lst;


            T obj = new T();
            //			r.ReadStart();
            obj.Load(r, 0);
            //			r.ReadEnd();
            return null;
        }
    }
}