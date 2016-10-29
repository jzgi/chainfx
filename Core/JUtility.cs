using System;
using System.Diagnostics;
using System.IO;

namespace Greatbone.Core
{
    
    public static class JUtility
    {

        public static JObj FileToJObj(string file)
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JParse jp = new JParse(bytes, bytes.Length);
                return (JObj)jp.Parse();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            return null;
        }

        public static P FileToObj<P>(string file) where P : IPersist, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JParse jp = new JParse(bytes, bytes.Length);
                JObj jo = (JObj)jp.Parse();
                if (jo != null)
                {
                    return jo.ToObj<P>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            return default(P);
        }

        public static P[] FileToArr<P>(string file) where P : IPersist, new()
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(file);
                JParse jp = new JParse(bytes, bytes.Length);
                JArr ja = (JArr)jp.Parse();
                if (ja != null)
                {
                    return ja.ToArr<P>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
            return null;
        }

    }

}