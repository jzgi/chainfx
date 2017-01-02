using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    public static class JsonUtility
    {
        public static JArr StringToJArr(string v)
        {
            JsonParse p = new JsonParse(v);
            return (JArr)p.Parse();
        }

        public static JObj StringToJObj(string v)
        {
            JsonParse p = new JsonParse(v);
            return (JObj)p.Parse();
        }

        public static D StringToDataObj<D>(string v, byte bits = 0) where D : IData, new()
        {
            JsonParse p = new JsonParse(v);
            JObj jobj = (JObj)p.Parse();
            return jobj.ToDataObj<D>(bits);
        }

        public static D[] StringToDataArr<D>(string v, byte z = 0) where D : IData, new()
        {
            JsonParse p = new JsonParse(v);
            JArr jarr = (JArr)p.Parse();
            return jarr.ToDataArr<D>(z);
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

        public static string DataObjToString<D>(D v, byte bits = 0) where D : IData
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string DataArrToString<D>(D[] v, byte bits = 0) where D : IData
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
                return (JObj)p.Parse();
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
                return (JArr)p.Parse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static D FileToDataObj<D>(string file) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                JObj jobj = (JObj)p.Parse();
                if (jobj != null)
                {
                    return jobj.ToDataObj<D>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(D);
        }

        public static D[] FileToDataArr<D>(string file) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                JArr jarr = (JArr)p.Parse();
                if (jarr != null)
                {
                    return jarr.ToDataArr<D>();
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