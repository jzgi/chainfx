using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Greatbone
{
    public static class DataUtility
    {
        /// <summary>
        /// Used in both client and server to parse received content into model.
        /// </summary>
        public static ISource ParseContent(string ctyp, byte[] buffer, int length, Type typ = null)
        {
            if (string.IsNullOrEmpty(ctyp)) return null;

            if (ctyp.StartsWith("application/x-www-form-urlencoded"))
            {
                return new FormParser(buffer, length).Parse();
            }
            if (ctyp.StartsWith("multipart/form-data; boundary="))
            {
                return new FormMpParser(buffer, length, ctyp.Substring(30)).Parse();
            }
            if (ctyp.StartsWith("application/json"))
            {
                return new JsonParser(buffer, length).Parse();
            }
            if (ctyp.StartsWith("application/xml"))
            {
                return new XmlParser(buffer, length).Parse();
            }
            if (ctyp.StartsWith("application/data-flow"))
            {
                return new FlowContext(buffer, length);
            }
            if (ctyp.StartsWith("text/"))
            {
                if (typ == typeof(JObj) || typ == typeof(JArr))
                {
                    return new JsonParser(buffer, length).Parse();
                }
                else if (typ == typeof(XElem))
                {
                    return new XmlParser(buffer, length).Parse();
                }
                else
                {
                    Text text = new Text();
                    for (int i = 0; i < length; i++)
                    {
                        text.Accept(buffer[i]);
                    }
                    return text;
                }
            }
            return null;
        }

        public static M StringTo<M>(string v) where M : class, ISource
        {
            Type t = typeof(M);
            if (t == typeof(JArr) || t == typeof(JObj))
            {
                return new JsonParser(v).Parse() as M;
            }
            else if (t == typeof(XElem))
            {
                return new XmlParser(v).Parse() as M;
            }
            else if (t == typeof(Form))
            {
                return new FormParser(v).Parse() as M;
            }
            return null;
        }

        public static D StringToObject<D>(string v, byte proj = 0x0f) where D : IData, new()
        {
            JObj jo = (JObj) new JsonParser(v).Parse();
            return jo.ToObject<D>(proj);
        }

        public static D[] StringToArray<D>(string v, byte proj = 0x0f) where D : IData, new()
        {
            JArr ja = (JArr) new JsonParser(v).Parse();
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

        public static T FileTo<T>(string file) where T : class, ISource
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);

                Type t = typeof(T);
                if (t == typeof(JArr) || t == typeof(JObj))
                {
                    return new JsonParser(bytes, bytes.Length).Parse() as T;
                }
                else if (t == typeof(XElem))
                {
                    return new XmlParser(bytes, bytes.Length).Parse() as T;
                }
                else if (t == typeof(Form))
                {
                    return new FormParser(bytes, bytes.Length).Parse() as T;
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
                JObj jo = (JObj) new JsonParser(bytes, bytes.Length).Parse();
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
                JArr ja = (JArr) new JsonParser(bytes, bytes.Length).Parse();
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
                JArr ja = (JArr) new JsonParser(bytes, bytes.Length).Parse();
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