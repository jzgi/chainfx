using System;

namespace Greatbone.Core
{
    public static class ISourceUtility
    {

        public static T ToObj<T>(this ISource sc, ushort x = 0) where T : IPersist, new()
        {
            T obj = new T();
            obj.Load(sc, x);
            return obj;
        }

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


        public static bool Got<V>(this ISource sc, ref V v, ushort x = 0) where V : IPersist, new()
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

        public static bool Got<V>(this ISource sc, ref V[] v, ushort x = 0) where V : IPersist, new()
        {
            return sc.Got(null, ref v, x);
        }


        //
        // GET SPECIFIC
        //

        public static bool GotBool(this ISource sc, bool def = false)
        {
            bool v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static short GotShort(this ISource sc, short def = 0)
        {
            short v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static int GotInt(this ISource sc, int def = 0)
        {
            int v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static long GotLong(this ISource sc, long def = 0)
        {
            long v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static decimal GotDecimal(this ISource sc, decimal def = 0)
        {
            decimal v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static Number GotNumber(this ISource sc, Number def = default(Number))
        {
            Number v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static DateTime GotDateTime(this ISource sc, DateTime def = default(DateTime))
        {
            DateTime v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static char[] GotChars(this ISource sc, char[] def = null)
        {
            char[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static string GotString(this ISource sc, string def = null)
        {
            string v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static byte[] GotBytes(this ISource sc, byte[] def = null)
        {
            byte[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static ArraySegment<byte> GotBytesSeg(this ISource sc, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static V GotObj<V>(this ISource sc, ushort x = 0, V def = default(V)) where V : IPersist, new()
        {
            V v = def;
            sc.Got(null, ref v, x);
            return v;
        }

        public static JObj GotJObj(this ISource sc, JObj def = null)
        {
            JObj v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static JArr GotJArr(this ISource sc, JArr def = null)
        {
            JArr v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static short[] GotShorts(this ISource sc, short[] def = null)
        {
            short[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static int[] GotInts(this ISource sc, int[] def = null)
        {
            int[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static long[] GotLongs(this ISource sc, long[] def = null)
        {
            long[] v = def;
            sc.Got(null, ref v);
            return v;
        }


        public static string[] GotStrings(this ISource sc, string[] def = null)
        {
            string[] v = def;
            sc.Got(null, ref v);
            return v;
        }

        public static V[] GotArr<V>(this ISource sc, ushort x = 0, V[] def = null) where V : IPersist, new()
        {
            V[] v = def;
            sc.Got(null, ref v, x);
            return v;
        }

        //
        // GET SPECIFIC BY NAME
        //

        public static bool GotBool(this ISource sc, string name, bool def = false)
        {
            bool v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static short GotShort(this ISource sc, string name, short def = 0)
        {
            short v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static int GotInt(this ISource sc, string name, int def = 0)
        {
            int v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static long GotLong(this ISource sc, string name, long def = 0)
        {
            long v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static decimal GotDecimal(this ISource sc, string name, decimal def = 0)
        {
            decimal v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static Number GotNumber(this ISource sc, string name, Number def = default(Number))
        {
            Number v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static DateTime GotDateTime(this ISource sc, string name, DateTime def = default(DateTime))
        {
            DateTime v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static char[] GotChars(this ISource sc, string name, char[] def = null)
        {
            char[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static string GotString(this ISource sc, string name, string def = null)
        {
            string v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static byte[] GotBytes(this ISource sc, string name, byte[] def = null)
        {
            byte[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static ArraySegment<byte> GotBytesSeg(this ISource sc, string name, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static T GotObj<T>(this ISource sc, string name, T def = default(T)) where T : IPersist, new()
        {
            T v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static JObj GotJObj(this ISource sc, string name, JObj def = null)
        {
            JObj v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static JArr GotJArr(this ISource sc, string name, JArr def = null)
        {
            JArr v = def;
            sc.Got(name, ref v);
            return v;
        }


        public static short[] GotShorts(this ISource sc, string name, short[] def = null)
        {
            short[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static int[] GotInts(this ISource sc, string name, int[] def = null)
        {
            int[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static long[] GotLongs(this ISource sc, string name, long[] def = null)
        {
            long[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static string[] GotStrings(this ISource sc, string name, string[] def = null)
        {
            string[] v = def;
            sc.Got(name, ref v);
            return v;
        }

        public static V[] GotArr<V>(this ISource sc, string name, V[] def = null) where V : IPersist, new()
        {
            V[] v = def;
            sc.Got(name, ref v);
            return v;
        }

    }

}