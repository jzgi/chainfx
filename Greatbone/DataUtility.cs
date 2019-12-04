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
        public static ISource ParseContent(string ctyp, byte[] buf, int len, Type typ = null)
        {
            if (string.IsNullOrEmpty(ctyp)) return null;

            if (ctyp.StartsWith("application/x-www-form-urlencoded"))
            {
                return new FormParser(buf, len).Parse();
            }

            if (ctyp.StartsWith("multipart/form-data; boundary="))
            {
                return new FormMpParser(buf, len, ctyp.Substring(30)).Parse();
            }

            if (ctyp.StartsWith("application/json"))
            {
                return new JsonParser(buf, len).Parse();
            }

            if (ctyp.StartsWith("application/xml"))
            {
                return new XmlParser(buf, len).Parse();
            }

            if (ctyp.StartsWith("text/"))
            {
                if (typ == typeof(JObj) || typ == typeof(JArr))
                {
                    return new JsonParser(buf, len).Parse();
                }
                else if (typ == typeof(XElem))
                {
                    return new XmlParser(buf, len).Parse();
                }
                else
                {
                    Text text = new Text();
                    for (int i = 0; i < len; i++)
                    {
                        text.Accept(buf[i]);
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
            var cnt = new JsonContent(4 * 1024, binary: false);
            try
            {
                cnt.Put(null, v, proj);
                return cnt.ToString();
            }
            finally
            {
                BufferUtility.Return(cnt.Buffer); // return buffer to pool
            }
        }

        public static string ToString<D>(D[] v, byte proj = 0x0f) where D : IData
        {
            var cnt = new JsonContent(4 * 1024, binary: false);
            try
            {
                cnt.Put(null, v, proj);
                return cnt.ToString();
            }
            finally
            {
                BufferUtility.Return(cnt.Buffer); // return buffer to pool
            }
        }

        public static string ToString<D>(List<D> v, byte proj = 0x0f) where D : IData
        {
            var cnt = new JsonContent(4 * 1024, binary: false);
            try
            {
                cnt.Put(null, v, proj);
                return cnt.ToString();
            }
            finally
            {
                BufferUtility.Return(cnt.Buffer); // return buffer to pool
            }
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
                var bytes = File.ReadAllBytes(file);
                var ja = (JArr) new JsonParser(bytes, bytes.Length).Parse();
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
                var bytes = File.ReadAllBytes(file);
                var ja = (JArr) new JsonParser(bytes, bytes.Length).Parse();
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