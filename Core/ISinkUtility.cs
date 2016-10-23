using System;

namespace Greatbone.Core
{
    public static class ISinkUtility
    {

        public static R PutNull<R>(this ISink<R> snk) where R : ISink<R>
        {
            return snk.PutNull(null);
        }

        public static R Put<R>(this ISink<R> snk, bool v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> snk, short v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, int v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, long v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, decimal v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, Number v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, DateTime v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, char[] v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, string v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, byte[] v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<R>(this ISink<R> snk, ArraySegment<byte> v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<P, R>(this ISink<R> snk, P v, uint x = 0) where P : IPersist where R : ISink<R>
        {
            return snk.Put(null, v, x);
        }

        public static R Put<R>(this ISink<R> snk, JObj v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> snk, JArr v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> snk, short[] v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> snk, int[] v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> snk, long[] v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }

        public static R Put<R>(this ISink<R> snk, string[] v) where R : ISink<R>
        {
            return snk.Put(null, v);
        }


        public static R Put<P, R>(this ISink<R> snk, P[] v, uint x = 0) where P : IPersist where R : ISink<R>
        {
            return snk.Put(null, v, x);
        }

    }
    
}