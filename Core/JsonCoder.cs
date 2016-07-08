using System.Collections.Generic;

namespace Greatbone.Core
{
    public class JsonCoder : Coder, ITarget, ISource
    {
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

        public bool Got<T>(string name, out List<T> value) where T : IPersist
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

        public void Put<T>(string name, List<T> value) where T : IPersist
        {
            foreach (T v in value)
            {
                v.To(this);
            }
        }
    }
}