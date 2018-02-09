using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    public static class DataUtility
    {
        /// <summary>
        /// Used in both client and server to parse received content into model.
        /// </summary>
        public static IDataInput ParseContent(string ctyp, byte[] buffer, int length, Type typ = null)
        {
            if (string.IsNullOrEmpty(ctyp)) return null;

            if (ctyp.StartsWith("application/x-www-form-urlencoded"))
            {
                return new FormParse(buffer, length).Parse();
            }
            if (ctyp.StartsWith("multipart/form-data; boundary="))
            {
                return new FormMpParse(buffer, length, ctyp.Substring(30)).Parse();
            }
            if (ctyp.StartsWith("application/json"))
            {
                return new JsonParse(buffer, length).Parse();
            }
            if (ctyp.StartsWith("application/xml"))
            {
                return new XmlParse(buffer, length).Parse();
            }
            if (ctyp.StartsWith("text/"))
            {
                if (typ == typeof(JObj) || typ == typeof(JArr))
                {
                    return new JsonParse(buffer, length).Parse();
                }
                else if (typ == typeof(XElem))
                {
                    return new XmlParse(buffer, length).Parse();
                }
                else
                {
                    Str str = new Str();
                    for (int i = 0; i < length; i++)
                    {
                        str.Accept(buffer[i]);
                    }
                    return str;
                }
            }
            return null;
        }

        public static M StringTo<M>(string v) where M : class, IDataInput
        {
            Type t = typeof(M);
            if (t == typeof(JArr) || t == typeof(JObj))
            {
                return new JsonParse(v).Parse() as M;
            }
            else if (t == typeof(XElem))
            {
                return new XmlParse(v).Parse() as M;
            }
            else if (t == typeof(Form))
            {
                return new FormParse(v).Parse() as M;
            }
            return null;
        }

        public static D StringToObject<D>(string v, byte proj = 0x0f) where D : IData, new()
        {
            JObj jo = (JObj) new JsonParse(v).Parse();
            return jo.ToObject<D>(proj);
        }

        public static D[] StringToArray<D>(string v, byte proj = 0x0f) where D : IData, new()
        {
            JArr ja = (JArr) new JsonParse(v).Parse();
            return ja.ToArray<D>(proj);
        }

        public static string ToString<D>(D v, byte proj = 0x0f) where D : IData
        {
            JsonContent cont = new JsonContent(false, 4 * 1024);
            cont.Put(null, v, proj);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ToString<D>(D[] v, byte proj = 0x0f) where D : IData
        {
            JsonContent cont = new JsonContent(false, 4 * 1024);
            cont.Put(null, v, proj);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ToString<D>(List<D> v, byte proj = 0x0f) where D : IData
        {
            JsonContent cont = new JsonContent(false, 4 * 1024);
            cont.Put(null, v, proj);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static T FileTo<T>(string file) where T : class, IDataInput
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);

                Type t = typeof(T);
                if (t == typeof(JArr) || t == typeof(JObj))
                {
                    return new JsonParse(bytes, bytes.Length).Parse() as T;
                }
                else if (t == typeof(XElem))
                {
                    return new XmlParse(bytes, bytes.Length).Parse() as T;
                }
                else if (t == typeof(Form))
                {
                    return new FormParse(bytes, bytes.Length).Parse() as T;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static D FileToObject<D>(string file, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JObj jo = (JObj) new JsonParse(bytes, bytes.Length).Parse();
                if (jo != null)
                {
                    return jo.ToObject<D>(proj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default;
        }

        public static D[] FileToArray<D>(string file, byte proj = 0x0f) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JArr ja = (JArr) new JsonParse(bytes, bytes.Length).Parse();
                if (ja != null)
                {
                    return ja.ToArray<D>(proj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static Map<K, D> FileToMap<K, D>(string file, byte proj = 0x0f, Func<D, K> keyer = null, Predicate<K> toper = null) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JArr ja = (JArr) new JsonParse(bytes, bytes.Length).Parse();
                if (ja != null)
                {
                    return ja.ToMap(proj, keyer, toper);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }
    }
}