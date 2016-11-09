using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{

    public static class JsonUtility
    {

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

        public static B FileToBean<B>(string file) where B : IBean, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                Obj obj = (Obj)p.Parse();
                if (obj != null)
                {
                    return obj.ToBean<B>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return default(B);
        }

        public static B[] FileToBeans<B>(string file) where B : IBean, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JsonParse p = new JsonParse(bytes, bytes.Length);
                Arr arr = (Arr)p.Parse();
                if (arr != null)
                {
                    return arr.ToBeans<B>();
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