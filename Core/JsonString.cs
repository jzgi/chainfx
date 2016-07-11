using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonString : IInput, IOutput
    {
        private string str;

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

        public bool Got(string name, out decimal value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got(string name, out string value)
        {
            throw new System.NotImplementedException();
        }

        public bool Got<T>(string name, out List<T> value) where T : IDump
        {
            throw new System.NotImplementedException();
        }

        public void PutStart()
        {
            throw new System.NotImplementedException();
        }

        public void PutEnd()
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, int value)
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, decimal value)
        {
            throw new System.NotImplementedException();
        }

        public void Put(string name, string value)
        {
            throw new System.NotImplementedException();
        }

        public void Put<T>(string name, List<T> value) where T : IDump
        {
            throw new System.NotImplementedException();
        }
    }
}