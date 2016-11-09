using System;

namespace Greatbone.Core
{
    public static class ISourceUtility
    {

        public static B ToBean<B>(this ISource src, byte z = 0) where B : IBean, new()
        {
            B bean = new B();
            bean.Load(src, z);
            return bean;
        }

        //
        // GOT
        //

        public static bool Get(this ISource src, ref bool v)
        {
            return src.Get(null, ref v);
        }

        public static bool Get(this ISource src, ref short v)
        {
            return src.Get(null, ref v);
        }

        public static bool Get(this ISource src, ref int v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref long v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref decimal v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref Number v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref DateTime v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref char[] v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref string v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref byte[] v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref ArraySegment<byte>? v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get<B>(this ISource src, ref B v, byte z = 0) where B : IBean, new()
        {
            return src.Get(null, ref v, z);
        }


        public static bool Get(this ISource src, ref Obj v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref Arr v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref short[] v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref int[] v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref long[] v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref string[] v)
        {
            return src.Get(null, ref v);
        }

        public static bool Get<B>(this ISource src, ref B[] v, byte z = 0) where B : IBean, new()
        {
            return src.Get(null, ref v, z);
        }


        //
        // GET SPECIFIC
        //

        public static bool GetBool(this ISource src)
        {
            bool v = false;
            src.Get(null, ref v);
            return v;
        }

        public static short GetShort(this ISource src)
        {
            short v = 0;
            src.Get(null, ref v);
            return v;
        }

        public static int GetInt(this ISource src)
        {
            int v = 0;
            src.Get(null, ref v);
            return v;
        }

        public static long GetLong(this ISource src)
        {
            long v = 0;
            src.Get(null, ref v);
            return v;
        }

        public static decimal GetDecimal(this ISource src)
        {
            decimal v = 0;
            src.Get(null, ref v);
            return v;
        }

        public static Number GetNumber(this ISource src)
        {
            Number v = default(Number);
            src.Get(null, ref v);
            return v;
        }

        public static DateTime GetDateTime(this ISource src)
        {
            DateTime v = default(DateTime);
            src.Get(null, ref v);
            return v;
        }

        public static char[] GetChars(this ISource src)
        {
            char[] v = null;
            src.Get(null, ref v);
            return v;
        }

        public static string GetString(this ISource src)
        {
            string v = null;
            src.Get(null, ref v);
            return v;
        }

        public static byte[] GetBytes(this ISource src)
        {
            byte[] v = null;
            src.Get(null, ref v);
            return v;
        }

        public static ArraySegment<byte>? GetBytesSeg(this ISource src)
        {
            ArraySegment<byte>? v = null;
            src.Get(null, ref v);
            return v;
        }

        public static B GetBean<B>(this ISource src, byte z = 0) where B : IBean, new()
        {
            B v = default(B);
            src.Get(null, ref v, z);
            return v;
        }

        public static Obj GetObj(this ISource src)
        {
            Obj v = null;
            src.Get(null, ref v);
            return v;
        }

        public static Arr GetArr(this ISource src)
        {
            Arr v = null;
            src.Get(null, ref v);
            return v;
        }

        public static short[] GetShorts(this ISource src)
        {
            short[] v = null;
            src.Get(null, ref v);
            return v;
        }

        public static int[] GetInts(this ISource src)
        {
            int[] v = null;
            src.Get(null, ref v);
            return v;
        }

        public static long[] GetLongs(this ISource src)
        {
            long[] v = null;
            src.Get(null, ref v);
            return v;
        }


        public static string[] GetStrings(this ISource src)
        {
            string[] v = null;
            src.Get(null, ref v);
            return v;
        }

        public static B[] GetBeans<B>(this ISource src, byte z = 0) where B : IBean, new()
        {
            B[] v = null;
            src.Get(null, ref v, z);
            return v;
        }

        //
        // GET SPECIFIC BY NAME
        //

        public static bool GetBool(this ISource src, string name)
        {
            bool v = false;
            src.Get(name, ref v);
            return v;
        }

        public static short GetShort(this ISource src, string name)
        {
            short v = 0;
            src.Get(name, ref v);
            return v;
        }

        public static int GetInt(this ISource src, string name)
        {
            int v = 0;
            src.Get(name, ref v);
            return v;
        }

        public static long GetLong(this ISource src, string name)
        {
            long v = 0;
            src.Get(name, ref v);
            return v;
        }

        public static decimal GetDecimal(this ISource src, string name)
        {
            decimal v = 0;
            src.Get(name, ref v);
            return v;
        }

        public static Number GetNumber(this ISource src, string name)
        {
            Number v = default(Number);
            src.Get(name, ref v);
            return v;
        }

        public static DateTime GetDateTime(this ISource src, string name)
        {
            DateTime v = default(DateTime);
            src.Get(name, ref v);
            return v;
        }

        public static char[] GetChars(this ISource src, string name)
        {
            char[] v = null;
            src.Get(name, ref v);
            return v;
        }

        public static string GetString(this ISource src, string name)
        {
            string v = null;
            src.Get(name, ref v);
            return v;
        }

        public static byte[] GetBytes(this ISource src, string name)
        {
            byte[] v = null;
            src.Get(name, ref v);
            return v;
        }

        public static ArraySegment<byte>? GetBytesSeg(this ISource src, string name)
        {
            ArraySegment<byte>? v = null;
            src.Get(name, ref v);
            return v;
        }

        public static B GetBean<B>(this ISource src, string name) where B : IBean, new()
        {
            B v = default(B);
            src.Get(name, ref v);
            return v;
        }

        public static Obj GetObj(this ISource src, string name)
        {
            Obj v = null;
            src.Get(name, ref v);
            return v;
        }

        public static Arr GetArr(this ISource src, string name)
        {
            Arr v = null;
            src.Get(name, ref v);
            return v;
        }


        public static short[] GetShorts(this ISource src, string name)
        {
            short[] v = null;
            src.Get(name, ref v);
            return v;
        }

        public static int[] GetInts(this ISource src, string name)
        {
            int[] v = null;
            src.Get(name, ref v);
            return v;
        }

        public static long[] GetLongs(this ISource src, string name)
        {
            long[] v = null;
            src.Get(name, ref v);
            return v;
        }

        public static string[] GetStrings(this ISource src, string name)
        {
            string[] v = null;
            src.Get(name, ref v);
            return v;
        }

        public static B[] GetBeans<B>(this ISource src, string name) where B : IBean, new()
        {
            B[] v = null;
            src.Get(name, ref v);
            return v;
        }

    }

}