using System;
using System.Collections.Generic;

namespace Greatbone.Core
{
    public static class IResultSetExtensions
    {

        public static List<T> ToList<T>(this IResultSet rs) where T : IPersist, new()
        {
            List<T> lst = new List<T>(32);

            while (rs.NextRow())
            {
                T obj = new T();
                obj.Load(rs);
                lst.Add(obj);
            }
            return lst;
        }

        //
        // GET SPECIFIC
        //

        public static bool GetBool(this IResultSet rs, bool def = false)
        {
            bool v = def;
            rs.Got(ref v);
            return v;
        }

        public static short GetShort(this IResultSet rs, short def = 0)
        {
            short v = def;
            rs.Got(ref v);
            return v;
        }

        public static int GetInt(this IResultSet rs, int def = 0)
        {
            int v = def;
            rs.Got(ref v);
            return v;
        }

        public static long GetLong(this IResultSet rs, long def = 0)
        {
            long v = def;
            rs.Got(ref v);
            return v;
        }

        public static decimal GetDecimal(this IResultSet rs, decimal def = 0)
        {
            decimal v = def;
            rs.Got(ref v);
            return v;
        }

        public static DateTime GetDateTime(this IResultSet rs, DateTime def = default(DateTime))
        {
            DateTime v = def;
            rs.Got(ref v);
            return v;
        }

        public static string GetString(this IResultSet rs, string def = null)
        {
            string v = def;
            rs.Got(ref v);
            return v;
        }

        public static T Get<T>(this IResultSet rs, T def = default(T)) where T : IPersist, new()
        {
            T v = def;
            rs.Got(ref v);
            return v;
        }

        public static List<T> GetList<T>(this IResultSet rs, List<T> def = null) where T : IPersist, new()
        {
            List<T> v = def;
            rs.Got(ref v);
            return v;
        }

        public static byte[] GetBytes(this IResultSet rs, byte[] def = null)
        {
            byte[] v = def;
            rs.Got(ref v);
            return v;
        }

        public static JObj GetObj(this IResultSet rs, JObj def = null)
        {
            JObj v = def;
            rs.Got(ref v);
            return v;
        }

        public static JArr GetArr(this IResultSet rs, JArr def = null)
        {
            JArr v = def;
            rs.Got(ref v);
            return v;
        }



    }

}