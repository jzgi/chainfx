using System;

namespace Greatbone.Core
{
    public static class ISourceUtility
    {
        //
        // GOT
        //

        public static bool Got(this ISource sc, ref bool v)
        {
            return sc.Got(null, ref v);
        }

        public static bool Got(this ISource sc, ref short v)
        {
            return sc.Got(null, ref v);
        }

        public static bool Got(this ISource sc, ref int v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref long v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref decimal v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref Number v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref DateTime v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref char[] v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref string v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref byte[] v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref ArraySegment<byte> v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got<T>(this ISource sc, ref T v, ushort x = 0xffff) where T : IPersist, new()
        {
            return sc.Got(null, ref v, x);
        }


        public static bool Got(this ISource sc, ref JObj v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref JArr v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref short[] v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref int[] v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref long[] v)
        {
            return sc.Got(null, ref v);
        }


        public static bool Got(this ISource sc, ref string[] v)
        {
            return sc.Got(null, ref v);
        }

        public static bool Got<T>(this ISource sc, ref T[] v, ushort x = 0xffff) where T : IPersist, new()
        {
            return sc.Got(null, ref v, x);
        }


        //
        // GET SPECIFIC
        //

        public static bool GetBool(this ISource sc, bool def = false)
        {
            bool v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static short GetShort(this ISource sc, short def = 0)
        {
            short v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static int GetInt(this ISource sc, int def = 0)
        {
            int v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static long GetLong(this ISource sc, long def = 0)
        {
            long v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static decimal GetDecimal(this ISource sc, decimal def = 0)
        {
            decimal v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static Number GetNumber(this ISource sc, Number def = default(Number))
        {
            Number v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static DateTime GetDateTime(this ISource sc, DateTime def = default(DateTime))
        {
            DateTime v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static char[] GetChars(this ISource sc, char[] def = null)
        {
            char[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static string GetString(this ISource sc, string def = null)
        {
            string v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static byte[] GetBytes(this ISource sc, byte[] def = null)
        {
            byte[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static ArraySegment<byte> GetBytesSeg(this ISource sc, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static T GetObj<T>(this ISource sc, T def = default(T)) where T : IPersist, new()
        {
            T v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static JObj GetJObj(this ISource sc, JObj def = null)
        {
            JObj v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static JArr GetJArr(this ISource sc, JArr def = null)
        {
            JArr v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static short[] GetShorts(this ISource sc, short[] def = null)
        {
            short[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static int[] GetInts(this ISource sc, int[] def = null)
        {
            int[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static long[] GetLongs(this ISource sc, long[] def = null)
        {
            long[] v = def;
            sc.Got(null, ref v);
            return v;
        }


        public static string[] GetStrings(this ISource sc, string[] def = null)
        {
            string[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static T[] GetArr<T>(this ISource sc, T[] def = null) where T : IPersist, new()
        {
            T[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        //
        // GET SPECIFIC BY NAME
        //

        public static bool GetBool(this ISource sc, string name, bool def = false)
        {
            bool v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static short GetShort(this ISource sc, string name, short def = 0)
        {
            short v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static int GetInt(this ISource sc, string name, int def = 0)
        {
            int v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static long GetLong(this ISource sc, string name, long def = 0)
        {
            long v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static decimal GetDecimal(this ISource sc, string name, decimal def = 0)
        {
            decimal v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static Number GetNumber(this ISource sc, string name, Number def = default(Number))
        {
            Number v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static DateTime GetDateTime(this ISource sc, string name, DateTime def = default(DateTime))
        {
            DateTime v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static char[] GetChars(this ISource sc, string name, char[] def = null)
        {
            char[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static string GetString(this ISource sc, string name, string def = null)
        {
            string v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static byte[] GetBytes(this ISource sc, string name, byte[] def = null)
        {
            byte[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static T GetObj<T>(this ISource sc, string name, T def = default(T)) where T : IPersist, new()
        {
            T v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static JObj GetJObj(this ISource sc, string name, JObj def = null)
        {
            JObj v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static JArr GetJArr(this ISource sc, string name, JArr def = null)
        {
            JArr v = def;
            sc.Got(name, ref v);
            return v;
        }


        public static short[] GetShorts(this ISource sc, string name, short[] def = null)
        {
            short[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static int[] GetInts(this ISource sc, string name, int[] def = null)
        {
            int[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static long[] GetLongs(this ISource sc, string name, long[] def = null)
        {
            long[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static string[] GetStrings(this ISource sc, string name, string[] def = null)
        {
            string[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static T[] GetArr<T>(this ISource sc, string name, T[] def = null) where T : IPersist, new()
        {
            T[] v = def;
            sc.Got(name, ref v);
            return v;
        }

    }

}