using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    public static class JsonUtility
    {
        public static JArr StringToJArr(string v)
        {
            JsonParse p = new JsonParse(v);
            return (JArr) p.Parse();
        }

        public static JObj StringToJObj(string v)
        {
            JsonParse p = new JsonParse(v);
            return (JObj) p.Parse();
        }

        public static D StringToObject<D>(string v, byte flags = 0) where D : IData, new()
        {
            JsonParse p = new JsonParse(v);
            JObj jobj = (JObj) p.Parse();
            return jobj.ToObject<D>(flags);
        }

        public static D[] StringToArray<D>(string v, byte flags = 0) where D : IData, new()
        {
            JsonParse p = new JsonParse(v);
            JArr jarr = (JArr) p.Parse();
            return jarr.ToArray<D>(flags);
        }

        public static string JArrToString(JArr v)
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string JObjToString(JObj v)
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ObjectToString<D>(D v, byte flags = 0) where D : IData
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ArrayToString<D>(D[] v, byte flags = 0) where D : IData
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ListToString<D>(List<D> v, byte flags = 0) where D : IData
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static JObj FileToJObj(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                return (JObj) p.Parse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static JArr FileToJArr(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                return (JArr) p.Parse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static D FileToObject<D>(string file) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                JObj jobj = (JObj) p.Parse();
                if (jobj != null)
                {
                    return jobj.ToObject<D>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(D);
        }

        public static D[] FileToArray<D>(string file) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                JArr jarr = (JArr) p.Parse();
                if (jarr != null)
                {
                    return jarr.ToArray<D>();
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