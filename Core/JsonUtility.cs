using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    public static class JsonUtility
    {
        public static Obj StrToObj(string json)
        {
            JsonParse p = new JsonParse(json);
            return (Obj)p.Parse();
        }

        public static Arr StrToArr(string json)
        {
            JsonParse p = new JsonParse(json);
            return (Arr)p.Parse();
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

        public static D FileToData<D>(string file) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                Obj obj = (Obj)p.Parse();
                if (obj != null)
                {
                    return obj.ToData<D>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(D);
        }

        public static D[] FileToDatas<D>(string file) where D : IData, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                Arr arr = (Arr)p.Parse();
                if (arr != null)
                {
                    return arr.ToDatas<D>();
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