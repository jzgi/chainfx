using System;
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


        public static R PutNull<R>(this ISink<R> sk) where R : ISink<R>
        {
            return sk.PutNull(null);
        }

        public static R Put<R>(this ISink<R> sk, bool v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> sk, short v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, int v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, long v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, decimal v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, Number v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, DateTime v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, char[] v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, string v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> sk, byte[] v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<T, R>(this ISink<R> sk, T v, int x = -1) where T : IPersist where R : ISink<R>
        {
            return sk.Put(null, v, x);
        }

        public static R Put<R>(this ISink<R> sk, JObj v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> sk, JArr v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> sk, short[] v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> sk, int[] v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> sk, long[] v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> sk, string[] v) where R : ISink<R>
        {
            return sk.Put(null, v);
        }


        public static R Put<T, R>(this ISink<R> sk, T[] v, int x = -1) where T : IPersist where R : ISink<R>
        {
            return sk.Put(null, v, x);
        }

    }
}