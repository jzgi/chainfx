using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class ISinkExtensions
    {
        public static void Save<R>(this ISink<R> w, params IPersist[] arr) where R : ISink<R>
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (i > 0)
                {
                    //					w.WriteSep();
                }
                arr[i].Save(w);
            }
        }

        public static void Save<T, R>(this ISink<R> w, IList<T> coll) where T : IPersist where R : ISink<R>
        {
            //			w.WriteArrayStart();
            int i = 0;
            foreach (var o in coll)
            {
                if (i++ > 0)
                {
                    //					w.WriteSep();
                }
                o.Save(w);
            }
            //			w.WriteArrayEnd();
        }

    }
}