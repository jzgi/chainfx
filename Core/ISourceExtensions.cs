using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class ISourceExtensions
    {

        public static T ToObj<T>(this ISource sc) where T : IPersist, new()
        {
            T obj = new T();
            obj.Load(sc);
            return obj;
        }

        //
        // GET SPECIFIC
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

        public static T[] GetArr<T>(this ISource sc, string name, T[] def = null) where T : IPersist, new()
        {
            T[] v = def;
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


    }

}