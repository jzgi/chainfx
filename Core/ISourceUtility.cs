using System;

namespace Greatbone.Core
{
    public static class ISourceUtility
    {

        public static T ToObj<T>(this ISource sc, byte x = 0xff) where T : IPersist, new()
        {
            T obj = new T();
            obj.Load(sc, x);
            return obj;
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


        public static bool Get(this ISource src, ref ArraySegment<byte> v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get<P>(this ISource src, ref P v, byte x = 0xff) where P : IPersist, new()
        {
            return src.Get(null, ref v, x);
        }


        public static bool Get(this ISource src, ref JObj v)
        {
            return src.Get(null, ref v);
        }


        public static bool Get(this ISource src, ref JArr v)
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

        public static bool Get<P>(this ISource src, ref P[] v, byte x = 0xff) where P : IPersist, new()
        {
            return src.Get(null, ref v, x);
        }


        //
        // GET SPECIFIC
        //

        public static bool GetBool(this ISource src, bool def = false)
        {
            bool v = def;
            src.Get(null, ref v);
            return v;
        }

        public static short GetShort(this ISource src, short def = 0)
        {
            short v = def;
            src.Get(null, ref v);
            return v;
        }

        public static int GetInt(this ISource src, int def = 0)
        {
            int v = def;
            src.Get(null, ref v);
            return v;
        }

        public static long GetLong(this ISource src, long def = 0)
        {
            long v = def;
            src.Get(null, ref v);
            return v;
        }

        public static decimal GetDecimal(this ISource src, decimal def = 0)
        {
            decimal v = def;
            src.Get(null, ref v);
            return v;
        }

        public static Number GetNumber(this ISource src, Number def = default(Number))
        {
            Number v = def;
            src.Get(null, ref v);
            return v;
        }

        public static DateTime GetDateTime(this ISource src, DateTime def = default(DateTime))
        {
            DateTime v = def;
            src.Get(null, ref v);
            return v;
        }

        public static char[] GetChars(this ISource src, char[] def = null)
        {
            char[] v = def;
            src.Get(null, ref v);
            return v;
        }

        public static string GetString(this ISource src, string def = null)
        {
            string v = def;
            src.Get(null, ref v);
            return v;
        }

        public static byte[] GetBytes(this ISource src, byte[] def = null)
        {
            byte[] v = def;
            src.Get(null, ref v);
            return v;
        }

        public static ArraySegment<byte> GetBytesSeg(this ISource src, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            src.Get(null, ref v);
            return v;
        }

        public static P GetObj<P>(this ISource src, byte x = 0xff, P def = default(P)) where P : IPersist, new()
        {
            P v = def;
            src.Get(null, ref v, x);
            return v;
        }

        public static JObj GetJObj(this ISource src, JObj def = null)
        {
            JObj v = def;
            src.Get(null, ref v);
            return v;
        }

        public static JArr GetJArr(this ISource src, JArr def = null)
        {
            JArr v = def;
            src.Get(null, ref v);
            return v;
        }

        public static short[] GetShorts(this ISource src, short[] def = null)
        {
            short[] v = def;
            src.Get(null, ref v);
            return v;
        }

        public static int[] GetInts(this ISource src, int[] def = null)
        {
            int[] v = def;
            src.Get(null, ref v);
            return v;
        }

        public static long[] GetLongs(this ISource src, long[] def = null)
        {
            long[] v = def;
            src.Get(null, ref v);
            return v;
        }


        public static string[] GetStrings(this ISource src, string[] def = null)
        {
            string[] v = def;
            src.Get(null, ref v);
            return v;
        }

        public static P[] GetArr<P>(this ISource src, byte x = 0xff, P[] def = null) where P : IPersist, new()
        {
            P[] v = def;
            src.Get(null, ref v, x);
            return v;
        }

        //
        // GET SPECIFIC BY NAME
        //

        public static bool GetBool(this ISource src, string name, bool def = false)
        {
            bool v = def;
            src.Get(name, ref v);
            return v;
        }

        public static short GetShort(this ISource src, string name, short def = 0)
        {
            short v = def;
            src.Get(name, ref v);
            return v;
        }

        public static int GetInt(this ISource src, string name, int def = 0)
        {
            int v = def;
            src.Get(name, ref v);
            return v;
        }

        public static long GetLong(this ISource src, string name, long def = 0)
        {
            long v = def;
            src.Get(name, ref v);
            return v;
        }

        public static decimal GetDecimal(this ISource src, string name, decimal def = 0)
        {
            decimal v = def;
            src.Get(name, ref v);
            return v;
        }

        public static Number GetNumber(this ISource src, string name, Number def = default(Number))
        {
            Number v = def;
            src.Get(name, ref v);
            return v;
        }

        public static DateTime GetDateTime(this ISource src, string name, DateTime def = default(DateTime))
        {
            DateTime v = def;
            src.Get(name, ref v);
            return v;
        }

        public static char[] GetChars(this ISource src, string name, char[] def = null)
        {
            char[] v = def;
            src.Get(name, ref v);
            return v;
        }

        public static string GetString(this ISource src, string name, string def = null)
        {
            string v = def;
            src.Get(name, ref v);
            return v;
        }

        public static byte[] GetBytes(this ISource src, string name, byte[] def = null)
        {
            byte[] v = def;
            src.Get(name, ref v);
            return v;
        }

        public static ArraySegment<byte> GetBytesSeg(this ISource src, string name, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            src.Get(name, ref v);
            return v;
        }

        public static P GetObj<P>(this ISource src, string name, P def = default(P)) where P : IPersist, new()
        {
            P v = def;
            src.Get(name, ref v);
            return v;
        }

        public static JObj GetJObj(this ISource src, string name, JObj def = null)
        {
            JObj v = def;
            src.Get(name, ref v);
            return v;
        }

        public static JArr GetJArr(this ISource src, string name, JArr def = null)
        {
            JArr v = def;
            src.Get(name, ref v);
            return v;
        }


        public static short[] GetShorts(this ISource src, string name, short[] def = null)
        {
            short[] v = def;
            src.Get(name, ref v);
            return v;
        }

        public static int[] GetInts(this ISource src, string name, int[] def = null)
        {
            int[] v = def;
            src.Get(name, ref v);
            return v;
        }

        public static long[] GetLongs(this ISource src, string name, long[] def = null)
        {
            long[] v = def;
            src.Get(name, ref v);
            return v;
        }

        public static string[] GetStrings(this ISource src, string name, string[] def = null)
        {
            string[] v = def;
            src.Get(name, ref v);
            return v;
        }

        public static P[] GetArr<P>(this ISource src, string name, P[] def = null) where P : IPersist, new()
        {
            P[] v = def;
            src.Get(name, ref v);
            return v;
        }

    }

}