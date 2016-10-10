using System;

namespace Greatbone.Core
{
    public static class ISinkExtensions
    {

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


        public static R Put<T, R>(this ISink<R> sk, T v, ushort x = 0xffff) where T : IPersist where R : ISink<R>
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


        public static R Put<T, R>(this ISink<R> sk, T[] v, ushort x = 0xffff) where T : IPersist where R : ISink<R>
        {
            return sk.Put(null, v, x);
        }

    }
}