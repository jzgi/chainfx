using System.Collections.Generic;

namespace Greatbone.Core
{
    public class Json : Outlet<Json>, ITarget
    {
        public Json(int initial) : base(initial)
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

        public void Put<T>(string name, List<T> value) where T : IPersistable
        {
            foreach (T v in value)
            {
                v.To(this);
            }
        }
    }
}