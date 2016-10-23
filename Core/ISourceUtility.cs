using System;

namespace Greatbone.Core
{
    public static class ISourceUtility
    {

        public static T ToObj<T>(this ISource sc, uint x = 0) where T : IPersist, new()
        {
            T obj = new T();
            obj.Load(sc, x);
            return obj;
        }

        //
        // GOT
        //

        public static bool Got(this ISource src, ref bool v)
        {
            return src.Got(null, ref v);
        }

        public static bool Got(this ISource src, ref short v)
        {
            return src.Got(null, ref v);
        }

        public static bool Got(this ISource src, ref int v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref long v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref decimal v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref Number v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref DateTime v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref char[] v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref string v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref byte[] v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref ArraySegment<byte> v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got<P>(this ISource src, ref P v, uint x = 0) where P : IPersist, new()
        {
            return src.Got(null, ref v, x);
        }


        public static bool Got(this ISource src, ref JObj v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref JArr v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref short[] v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref int[] v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref long[] v)
        {
            return src.Got(null, ref v);
        }


        public static bool Got(this ISource src, ref string[] v)
        {
            return src.Got(null, ref v);
        }

        public static bool Got<P>(this ISource src, ref P[] v, uint x = 0) where P : IPersist, new()
        {
            return src.Got(null, ref v, x);
        }


        //
        // GET SPECIFIC
        //

        public static bool GotBool(this ISource src, bool def = false)
        {
            bool v = def;
            src.Got(null, ref v);
            return v;
        }

        public static short GotShort(this ISource src, short def = 0)
        {
            short v = def;
            src.Got(null, ref v);
            return v;
        }

        public static int GotInt(this ISource src, int def = 0)
        {
            int v = def;
            src.Got(null, ref v);
            return v;
        }

        public static long GotLong(this ISource src, long def = 0)
        {
            long v = def;
            src.Got(null, ref v);
            return v;
        }

        public static decimal GotDecimal(this ISource src, decimal def = 0)
        {
            decimal v = def;
            src.Got(null, ref v);
            return v;
        }

        public static Number GotNumber(this ISource src, Number def = default(Number))
        {
            Number v = def;
            src.Got(null, ref v);
            return v;
        }

        public static DateTime GotDateTime(this ISource src, DateTime def = default(DateTime))
        {
            DateTime v = def;
            src.Got(null, ref v);
            return v;
        }

        public static char[] GotChars(this ISource src, char[] def = null)
        {
            char[] v = def;
            src.Got(null, ref v);
            return v;
        }

        public static string GotString(this ISource src, string def = null)
        {
            string v = def;
            src.Got(null, ref v);
            return v;
        }

        public static byte[] GotBytes(this ISource src, byte[] def = null)
        {
            byte[] v = def;
            src.Got(null, ref v);
            return v;
        }

        public static ArraySegment<byte> GotBytesSeg(this ISource src, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            src.Got(null, ref v);
            return v;
        }

        public static P GotObj<P>(this ISource src, uint x = 0, P def = default(P)) where P : IPersist, new()
        {
            P v = def;
            src.Got(null, ref v, x);
            return v;
        }

        public static JObj GotJObj(this ISource src, JObj def = null)
        {
            JObj v = def;
            src.Got(null, ref v);
            return v;
        }

        public static JArr GotJArr(this ISource src, JArr def = null)
        {
            JArr v = def;
            src.Got(null, ref v);
            return v;
        }

        public static short[] GotShorts(this ISource src, short[] def = null)
        {
            short[] v = def;
            src.Got(null, ref v);
            return v;
        }

        public static int[] GotInts(this ISource src, int[] def = null)
        {
            int[] v = def;
            src.Got(null, ref v);
            return v;
        }

        public static long[] GotLongs(this ISource src, long[] def = null)
        {
            long[] v = def;
            src.Got(null, ref v);
            return v;
        }


        public static string[] GotStrings(this ISource src, string[] def = null)
        {
            string[] v = def;
            src.Got(null, ref v);
            return v;
        }

        public static P[] GotArr<P>(this ISource src, uint x = 0, P[] def = null) where P : IPersist, new()
        {
            P[] v = def;
            src.Got(null, ref v, x);
            return v;
        }

        //
        // GET SPECIFIC BY NAME
        //

        public static bool GotBool(this ISource src, string name, bool def = false)
        {
            bool v = def;
            src.Got(name, ref v);
            return v;
        }

        public static short GotShort(this ISource src, string name, short def = 0)
        {
            short v = def;
            src.Got(name, ref v);
            return v;
        }

        public static int GotInt(this ISource src, string name, int def = 0)
        {
            int v = def;
            src.Got(name, ref v);
            return v;
        }

        public static long GotLong(this ISource src, string name, long def = 0)
        {
            long v = def;
            src.Got(name, ref v);
            return v;
        }

        public static decimal GotDecimal(this ISource src, string name, decimal def = 0)
        {
            decimal v = def;
            src.Got(name, ref v);
            return v;
        }

        public static Number GotNumber(this ISource src, string name, Number def = default(Number))
        {
            Number v = def;
            src.Got(name, ref v);
            return v;
        }

        public static DateTime GotDateTime(this ISource src, string name, DateTime def = default(DateTime))
        {
            DateTime v = def;
            src.Got(name, ref v);
            return v;
        }

        public static char[] GotChars(this ISource src, string name, char[] def = null)
        {
            char[] v = def;
            src.Got(name, ref v);
            return v;
        }

        public static string GotString(this ISource src, string name, string def = null)
        {
            string v = def;
            src.Got(name, ref v);
            return v;
        }

        public static byte[] GotBytes(this ISource src, string name, byte[] def = null)
        {
            byte[] v = def;
            src.Got(name, ref v);
            return v;
        }

        public static ArraySegment<byte> GotBytesSeg(this ISource src, string name, ArraySegment<byte> def = default(ArraySegment<byte>))
        {
            ArraySegment<byte> v = def;
            src.Got(name, ref v);
            return v;
        }

        public static P GotObj<P>(this ISource src, string name, P def = default(P)) where P : IPersist, new()
        {
            P v = def;
            src.Got(name, ref v);
            return v;
        }

        public static JObj GotJObj(this ISource src, string name, JObj def = null)
        {
            JObj v = def;
            src.Got(name, ref v);
            return v;
        }

        public static JArr GotJArr(this ISource src, string name, JArr def = null)
        {
            JArr v = def;
            src.Got(name, ref v);
            return v;
        }


        public static short[] GotShorts(this ISource src, string name, short[] def = null)
        {
            short[] v = def;
            src.Got(name, ref v);
            return v;
        }

        public static int[] GotInts(this ISource src, string name, int[] def = null)
        {
            int[] v = def;
            src.Got(name, ref v);
            return v;
        }

        public static long[] GotLongs(this ISource src, string name, long[] def = null)
        {
            long[] v = def;
            src.Got(name, ref v);
            return v;
        }

        public static string[] GotStrings(this ISource src, string name, string[] def = null)
        {
            string[] v = def;
            src.Got(name, ref v);
            return v;
        }

        public static P[] GotArr<P>(this ISource src, string name, P[] def = null) where P : IPersist, new()
        {
            P[] v = def;
            src.Got(name, ref v);
            return v;
        }

    }

}