using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    public static class DataInputUtility
    {
        ///
        /// Used in both client and server to parse received content into model.
        ///
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
                    Str txt = new Str();
                    for (int i = 0; i < length; i++)
                    {
                        txt.Accept(buffer[i]);
                    }
                    return txt;
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

        public static D StringToData<D>(string v, ushort proj = 0) where D : IData, new()
        {
            JObj jo = (JObj) new JsonParse(v).Parse();
            return jo.ToData<D>(proj);
        }

        public static D[] StringToDatas<D>(string v, ushort proj = 0) where D : IData, new()
        {
            JArr ja = (JArr) new JsonParse(v).Parse();
            return ja.ToDatas<D>(proj);
        }

        public static string ToString<D>(D v, ushort proj = 0) where D : IData
        {
            JsonContent cont = new JsonContent(false, 4 * 1024);
            cont.Put(null, v, proj);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ToString<D>(D[] v, ushort proj = 0) where D : IData
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

        public static D FileToData<D>(string file, ushort proj = 0) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JObj jo = (JObj) new JsonParse(bytes, bytes.Length).Parse();
                if (jo != null)
                {
                    return jo.ToData<D>(proj);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(D);
        }

        public static D[] FileToDatas<D>(string file, ushort proj = 0) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JArr ja = (JArr) new JsonParse(bytes, bytes.Length).Parse();
                if (ja != null)
                {
                    return ja.ToDatas<D>(proj);
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