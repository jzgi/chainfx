using System.Collections.Generic;
using System.IO;

namespace Greatbone.Core
{
    public class JsonBin : Bin, IDataOutput, IDataInput
    {
        private string str;

        private Stream stream;


        private int pos;

        public bool GotStart()
        {
            while (pos < str.Length)
            {
                if (str[pos++] == '{')
                {
                    return true;
                }
            }
            return false;
        }

        public bool GotEnd()
        {
            while (pos < str.Length)
            {
                if (str[pos++] == '}')
                {
                    return true;
                }
            }
            return false;
        }

        public bool Got(string name, out int value)
        {
            while (pos < str.Length)
            {
                if (str[pos++] == '}')
                {
                    value = 1;
                    return true;
                }
            }
            value = 0;
            return false;
        }


        public JsonBin(int initial) : base(initial)
        {
        }

        public override string ContentType => "application/json";


        public bool Got(string name, out decimal value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got<T>(string name, out List<T> value) where T : IData
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, int value)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(' ');
            Put(':');
            Put(' ');
            Put(value);
        }

        public void Put(string name, decimal value)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(' ');
            Put(':');
            Put(' ');
            Put(value);
        }

        public void Put(string name, string value)
        {
            Put('"');
            Put(name);
            Put('"');
            Put(' ');
            Put(':');
            Put(' ');
            Put(value);
        }

        public void Put<T>(string name, List<T> value) where T : IData
        {
            foreach (T v in value)
            {
                v.To(this, -1);
            }
        }

        public void PutStart()
        {
            throw new System.NotImplementedException();
        }

        public void PutEnd()
        {
            throw new System.NotImplementedException();
        }
    }
}