using System.Collections.Generic;
using System.IO;

namespace Greatbone.Core
{
    public class JsonCoder : Coder, IOutput, IInput
    {
        private string str;

        private Stream stream;

        public JsonCoder(int initial) : base(initial)
        {
        }

        public override string ContentType => "application/json";

        public bool Got(string name, out int value)
        {
            throw new System.NotImplementedException();
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

        public void Put<T>(string name, List<T> value) where T : IDump
        {
            foreach (T v in value)
            {
                v.To(this);
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

        public bool GotStart()
        {
            throw new System.NotImplementedException();
        }

        public bool GotEnd()
        {
            throw new System.NotImplementedException();
        }
    }
}