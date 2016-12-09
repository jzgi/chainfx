using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    public static class JsonUtility
    {
        public static Arr StringToArr(string v)
        {
            JsonParse p = new JsonParse(v);
            return (Arr)p.Parse();
        }

        public static Obj StringToObj(string v)
        {
            JsonParse p = new JsonParse(v);
            return (Obj)p.Parse();
        }

        public static D[] StringToDats<D>(string v, byte z = 0) where D : IDat, new()
        {
            JsonParse p = new JsonParse(v);
            Arr arr = (Arr)p.Parse();
            return arr.ToDats<D>(z);
        }

        public static D StringToDat<D>(string v, byte z = 0) where D : IDat, new()
        {
            JsonParse p = new JsonParse(v);
            Obj obj = (Obj)p.Parse();
            return obj.ToDat<D>(z);
        }

        public static string ArrToString(Arr v)
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string ObjToString(Obj v)
        {
            JsonContent cont = new JsonContent(false, true, 4 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string DatsToString<D>(D[] v, byte z = 0) where D : IDat
        {
            JsonContent cont = new JsonContent(false, true, 8 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static string DatToString<D>(D v, byte z = 0) where D : IDat
        {
            JsonContent cont = new JsonContent(false, true, 8 * 1024);
            cont.Put(null, v);
            string str = cont.ToString();
            BufferUtility.Return(cont); // return buffer to pool
            return str;
        }

        public static Obj FileToObj(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                return (Obj)p.Parse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static Arr FileToArr(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                return (Arr)p.Parse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static D FileToDat<D>(string file) where D : IDat, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                Obj obj = (Obj)p.Parse();
                if (obj != null)
                {
                    return obj.ToDat<D>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(D);
        }

        public static D[] FileToDats<D>(string file) where D : IDat, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                Arr arr = (Arr)p.Parse();
                if (arr != null)
                {
                    return arr.ToDats<D>();
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